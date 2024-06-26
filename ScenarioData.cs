﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;

namespace WDS_Dispatches {
    public class ScenarioReader {
        private string[] _lines;
        private int _index;

        public ScenarioReader(string[] lines) {
            _lines = lines;
            _index = 0;
        }

        public int Position() {
            return _index;
        }

        public void SetPosition(int index) {
            _index = index;
        }

        public void Reset() {
            _index = 0;
        }

        public string ReadLine() {
            if(_index < _lines.Length) {
                return _lines[_index++];
            }

            return null;
        }
    }

    public class ScenarioData {
        private static List<string> Months;

        private static int ERA_PREMODERN = 0;
        private static int ERA_MODERN = 1;

        private List<Dictionary<string, object>> _armies;
        private Dictionary<string, Dictionary<string, object>> _unitData;
        private List<string> _objectives;
        private List<string> _nations;
        private Dictionary<string, bool> _unitPresent;
        private System.Windows.Forms.TreeView _tvRecip;
        private System.Windows.Forms.TreeView _tvSender;
        private bool _loadedCorrectly;
        private string _scenarioName;
        private int _armyPlayingIndex;
        private int _armyPlayingIndexOriginal;
        private int _currentTurn;
        private string _currentDay;
        private string _currentTime;
        private int _year;
        private int _maxTurns;
        private string _filename;
        private string _oobFilename;
        private string _shortFilename;
        private string _mapFilename;
        private string _pdtFilename;
        private bool _hasUpdated;
        private int _scenarioEra;
        private bool _isPBEM;

        public int MapWidth {get; set;}
        public int MapHeight { get; set;}

        public bool LoadedCorrectly() {
            return _loadedCorrectly;
        }

        public bool HasUpdated() {
            if(_hasUpdated) {
                _hasUpdated = false;
                return true;
            }

            return false;
        }

        public string GetScenarioName() {
            return _scenarioName;
        }

        public string GetScenarioDate() {
            return _currentDay;
        }

        public string GetScenarioTime() {
            return _currentTime;
        }

        public List<string> GetNations() {
            return _nations;
        }

        public void SetNation(string nation) {
            if (_nations.Contains(nation)) {
                _armyPlayingIndex = _nations.IndexOf(nation);
            }
        }

        public string GetNation() {
            return _nations[_armyPlayingIndex];
        }

        public bool IsPBEM() { return _isPBEM; }

        public int GetCurrentTurn() { return _currentTurn; }

        public int GetMaxTurns() { return _maxTurns; }

        public string GetFilenameFullPath() { return _filename; }

        public string GetFilename() { return _shortFilename;  }

        public string GetOOBFilename() { return _oobFilename; }

        public string GetPDTFilename() { return _pdtFilename; }

        public string GetMapFilename() { return _mapFilename; }

        public ScenarioData() {
            _armies = new List<Dictionary<string, object>>();
            _unitData = new Dictionary<string, Dictionary<string, object>>();
            _objectives = new List<string>();
            _nations = new List<string>();
            _loadedCorrectly = false;

            if (ScenarioData.Months == null) {
                ScenarioData.Months = new List<string> {
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June",
                    "July",
                    "August",
                    "September",
                    "October",
                    "November",
                    "December"
                };
            }

            _hasUpdated = false;
        }

        public Dictionary<string, object> GetUnitDataByNodeName(string node_name) {
            if (_unitData.ContainsKey(node_name)) {
                return _unitData[node_name];
            }

            return null;
        }

        public List<string> GetObjectives() { return _objectives; }

        private static string GetMonthNameByIndex(int index) {
            if (index > 0 && index < 13) {
                return ScenarioData.Months[index-1];
            }

            return "Invalid";
        }

        public static int ReadNum(ScenarioReader f) {
            string line = f.ReadLine();
            if (line != null) {
                return int.Parse(
                    line.Trim()
                );
            }
            return -1;
        }

        public static string ReadString(ScenarioReader f) {
            string line = f.ReadLine();
            if (line != null) {
                return line.Trim();
            }
            return null;
        }

        public static Dictionary<string, object> ReadUnit(ScenarioReader f, string line) {
            string[] items = line.Split();
            string unitType = items[0].ToLower();

            Dictionary<string, string> abbrevToType = new Dictionary<string, string> {
                { "u", "Armed Unit" },
                { "s", "Supply"     },
                { "g", "Ship"       } 
            };

            string unitName = "";
            for (int i = 12; i < items.Length; i++) {
                unitName += items[i] + " ";
            }
            unitName = unitName.Trim();

            return new Dictionary<string, object> {
                { "name",       unitName                    },
                { "type",       "Unit"                      },
                { "subtype",    abbrevToType[unitType]      }
            };
        }

        public static Dictionary<string, object> ReadLeader(ScenarioReader f, string line) {
            string[] items = line.Split();
            string leaderName = "";
            // Find where the leader's name starts
            int nameIndex = 5;
            for(int i = 1; i < items.Length; i++) {
                try {
                    int test = int.Parse(items[i]);
                } catch(Exception) {
                    nameIndex = i;
                    break;
                }
            }
            for (int i = nameIndex; i < items.Length; i++) {
                leaderName += items[i] + " ";
            }
            leaderName = leaderName.Trim();

            return new Dictionary<string, object> {
                { "name", leaderName },
                { "type", "Leader" }
            };
        }

        public static Dictionary<string, object> ReadFormation(ScenarioReader f, string line) {
            char formType = char.ToLower(line[0]);
            string formName = line.Substring(2);
            Dictionary<char, string> formKeys = new Dictionary<char, string> {
                { 'c', "Corps" },
                { 'd', "Division" },
                { 'w', "Wing" },
                { 'b', "Brigade" }
            };

            List<Dictionary<string, object>> units = new List<Dictionary<string, object>>();

            string readLine = ReadString(f);
            if (readLine.ToLower() != "begin") {
                throw new Exception("Expected 'begin'");
            }

            readLine = ReadString(f);
            while (readLine.ToLower() != "end") {
                Dictionary<string, object> oob_object;

                char lineType = char.ToLower(readLine[0]);
                if (lineType == 'l') {
                    oob_object = ReadLeader(f, readLine);
                } else if (lineType == 'u' || lineType == 's' || lineType == 'g') {
                    oob_object = ReadUnit(f, readLine);
                } else {
                    oob_object = ReadFormation(f, readLine);
                }
                units.Add(oob_object);

                readLine = ReadString(f);
            }

            return new Dictionary<string, object> {
                { "name",   formName            },
                { "type",   formKeys[formType]  },
                { "units",  units               }
            };
        }

        public bool ReadArmy(ScenarioReader f, string line) {
            string[] items = line.Split();
            while (items.Length < 2) {
                line = ReadString(f);
                if(line == null) {
                    return true;
                }

                items = line.Split();
            }

            string[] formation_types = { "a", "c", "d", "w", "b" };

            if (!formation_types.Contains(items[1].ToLower())) {
                // TODO: might be a supply unit
                return true;
            }

            // Add nation to list of combatants
            string nation = items[0];
            if(!_nations.Contains(nation)) {
                _nations.Add(nation);
            }

            string armyName = "";
            for (int i = 2; i < items.Length; i++) {
                armyName += items[i] + " ";
            }
            armyName = armyName.Trim();

            List<Dictionary<string, object>> units = new List<Dictionary<string, object>>();

            string readLine = ReadString(f);
            if (readLine.ToLower() != "begin") {
                return false;
            }

            readLine = ReadString(f);

            while (readLine.ToLower() != "end") {
                char lineType = char.ToLower(readLine[0]);
                if (lineType == 'l') {
                    units.Add(
                        ReadLeader(f, readLine)
                    );
                } else if (lineType == 'u' || lineType == 's' || lineType == 'g') { // Unit or supply
                    units.Add(
                        ReadUnit(f, readLine)
                    );
                } else {
                    units.Add(
                        ReadFormation(f, readLine)
                    );
                }
                readLine = ReadString(f);
            }

            _armies.Add(
                new Dictionary<string, object> {
                    { "name", armyName },
                    { "type", "Army" },
                    { "nation", nation },
                    { "units", units }
                }
            );

            return true;
        }

        public bool ReadOOB(string filename) {
            _armies.Clear();

            string[] all_lines;
            if (File.Exists(filename)) {
                all_lines = File.ReadAllLines(
                    filename,
                    Encoding.GetEncoding("ISO-8859-1")
                );
            } else {
                MessageBox.Show(
                    "Can't find scenario order-of-battle at " + filename + "!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
            
            ScenarioReader oob = new ScenarioReader(all_lines);

            int oobNum = ReadNum(oob);
            if (_scenarioEra == ERA_MODERN) {
                int oobId = ReadNum(oob);
            }

            _nations.Clear();

            string line = ReadString(oob);
            while (line != null) {
                if(!ReadArmy(oob, line)) {
                    return false;
                }
                line = ReadString(oob);
            }

            PopulateArmies();

            return true;
        }

        public void ParseScenarioHeader(ScenarioReader scenario) {
            int fileVersion;
            string first_line = ReadString(scenario);

            if(first_line.Contains("PEM Header")) {
                _isPBEM = true;
                ReadString(scenario); // Not sure what this is, but we don't use it
                fileVersion = ReadNum(scenario);
            } else {
                _isPBEM = false;
                fileVersion = int.Parse(first_line);
            }

            _scenarioName = ReadString(scenario);

            if (_scenarioName[_scenarioName.Length-1] == ',') {
                // Trim extra commas
                _scenarioName = _scenarioName.Substring(0, _scenarioName.Length - 1);
            }

            string scenarioData = ReadString(scenario);

            string[] items = scenarioData.Split();
            int year = int.Parse(items[0]);
            int month, day, hour, minute, phase;
            int remainingHeader;

            if (year < 1890) {
                month = int.Parse(items[1]);
                day = int.Parse(items[2]);
                hour = int.Parse(items[3]);
                minute = int.Parse(items[4]);
                _armyPlayingIndex = _armyPlayingIndexOriginal = int.Parse(items[5]);
                phase = int.Parse(items[6]);
                _currentTurn = int.Parse(items[7]);
                _maxTurns = int.Parse(items[8]);

                remainingHeader = 9;
                _scenarioEra = ERA_PREMODERN;
            } else {
                month = int.Parse(items[1]);
                day = int.Parse(items[2]);
                hour = int.Parse(items[3]);
                _armyPlayingIndex = int.Parse(items[4]);
                phase = int.Parse(items[5]);
                _currentTurn = int.Parse(items[6]);
                _maxTurns = int.Parse(items[7]);
                
                minute = 0;
                remainingHeader = 12;
                _scenarioEra = ERA_MODERN;
            }

            string dayTime = "AM";
            if (hour > 11) {
                dayTime = "PM";
                if (hour > 12) {
                    hour -= 12;
                }
            }
            string minStr = minute.ToString();
            if (minute < 10) {
                minStr = "0" + minStr;
            }
            _currentTime = hour + ":" + minStr + " " + dayTime;

            _currentDay = GetMonthNameByIndex(month) + " " + day + ", " + year;
            _year = year;

            string line;
            for (int i = 0; i < remainingHeader; i++) {
                line = ReadString(scenario);
                if (line.IndexOf(".oob") != -1) {
                    _oobFilename = line;
                } else if(line.IndexOf(".map") != -1) {
                    _mapFilename = line;
                } else if(line.IndexOf(".pdt") != -1) {
                    _pdtFilename = line;
                }
            }
        }

        public bool ReadMap(string basepath, string filename) {
            string map_path = basepath + filename;
            string[] all_lines;

            if (File.Exists(map_path)) {
                all_lines = File.ReadAllLines(
                    map_path,
                    Encoding.GetEncoding("ISO-8859-1")
                );
            } else {
                MessageBox.Show(
                    "Can't find scenario map at " + map_path + "!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }

            ScenarioReader mapfile = new ScenarioReader(all_lines);

            int x = -1, 
                y = -1,
                width = -1,
                height = -1;

            int map_version = ReadNum(mapfile);
            
            if(map_version == 0) {
                // Submap
                bool parsed_correctly = true;

                string new_mapfile = ReadString(mapfile);
                string[] dims = ReadString(mapfile).Split();
                if (dims.Length < 4) {
                    parsed_correctly = false;
                } else {
                    try {
                        x = int.Parse(dims[0]);
                        y = int.Parse(dims[1]);
                        width = int.Parse(dims[2]);
                        height = int.Parse(dims[3]);
                    } catch(Exception) {
                        parsed_correctly = false;
                    }
                }

                map_path = basepath + new_mapfile;

                if (!parsed_correctly) {
                    MessageBox.Show(
                        "Can't read scenario submap dimensions at " + map_path + "!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                if (File.Exists(map_path)) {
                    all_lines = File.ReadAllLines(
                        basepath + new_mapfile,
                        Encoding.GetEncoding("ISO-8859-1")
                    );
                } else {
                    MessageBox.Show(
                        "Can't find scenario submap at " + map_path + "!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                mapfile = new ScenarioReader(all_lines);
            }
            mapfile.Reset();
            
            if(!ReadScenarioMap(mapfile, x, y, width, height)) {
                MessageBox.Show(
                    "Can't read scenario map at " + map_path + "!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }

            return true;
        }

        public bool ReadScenarioMap(ScenarioReader map, int x, int y, int w, int h) {
            int fileVersion = ReadNum(map);
            string[] dimensions = ReadString(map).Split();

            int width = -1;
            int height = -1;
            bool parsed_correctly = true;

            if (dimensions.Length < 2) {
                parsed_correctly = false;
            } else {
                try {
                    width = int.Parse(dimensions[0]);
                    height = int.Parse(dimensions[1]);
                } catch (Exception) {
                    parsed_correctly = false;
                }
            }

            MapWidth = width;
            MapHeight = height;

            if(!parsed_correctly) {
                return false;
            }

            string map_metadata = ReadString(map);

            // Find and parse objective lines
            _objectives.Clear();
            // Finds a string preceded by two floats and three or more ints
            string objective_pattern = @"^(-?[0-9]+.[0-9]+\s+){2}(-?[0-9]+\s+){3,}([\S|\s]+)$";
            Regex regex = new Regex(objective_pattern, RegexOptions.Compiled);
            do {
                string line = ReadString(map);
                if (line == null) {
                    break;
                }

                try {
                    Match match = regex.Match(line);
                    if (match.Success && match.Groups.Count == 4) { // It's an objective line
                        // The objective name will always be the final group
                        string objective_name = match.Groups[3].Captures[0].ToString().Trim();

                        string[] items = line.Split();

                        float obj_x = -1.0f;
                        float obj_y = -1.0f;
                        try {
                            obj_x = float.Parse(items[0], CultureInfo.InvariantCulture.NumberFormat);
                            obj_y = float.Parse(items[1], CultureInfo.InvariantCulture.NumberFormat);
                        } catch (Exception) {
                            // If there's an error parsing the objective's coordinates, skip it
                            continue;
                        }

                        if (x != -1) {
                            // Determine whether the objective is within the submap
                            if (obj_x < x || obj_x > (x + w) || obj_y < y || obj_y > (y + h)) {
                                continue;
                            }
                        }

                        // No duplicate objective names
                        if (!_objectives.Contains(objective_name)) {
                            _objectives.Add(objective_name);
                        }
                    }
                } catch(Exception) {}
                
            } while (true);

            return true;
        }

        public bool CheckScenarioHeader(ScenarioReader scenario) {
            int fileVersion;
            string first_line = ReadString(scenario).Trim();

            if (first_line.Contains("PEM Header")) {
                if (!_isPBEM) {
                    return false;
                }

                ReadString(scenario); // Not sure what this is, but we don't use it
                fileVersion = ReadNum(scenario);
            } else {
                if(_isPBEM) {
                    return false;
                }

                fileVersion = int.Parse(first_line);
            }

            string scenarioName = ReadString(scenario);

            if (scenarioName[scenarioName.Length - 1] == ',') {
                // Trim extra commas
                scenarioName = scenarioName.Substring(0, scenarioName.Length - 1);
            }
            if (_scenarioName != scenarioName) {
                return false;
            }
            
            string scenarioData = ReadString(scenario);

            string[] items = scenarioData.Split();
            int year = int.Parse(items[0]);
            int month, day, hour, minute, phase, armyPlaying, currentTurn, maxTurns;
            int remainingHeader;
            int scenarioEra;

            if (year < 1890) {
                month = int.Parse(items[1]);
                day = int.Parse(items[2]);
                hour = int.Parse(items[3]);
                minute = int.Parse(items[4]);
                armyPlaying = int.Parse(items[5]);
                phase = int.Parse(items[6]);
                currentTurn = int.Parse(items[7]);
                maxTurns = int.Parse(items[8]);

                remainingHeader = 9;
                scenarioEra = ERA_PREMODERN;
            } else {
                month = int.Parse(items[1]);
                day = int.Parse(items[2]);
                hour = int.Parse(items[3]);
                armyPlaying = int.Parse(items[4]);
                phase = int.Parse(items[5]);
                currentTurn = int.Parse(items[6]);
                maxTurns = int.Parse(items[7]);

                minute = 0;
                remainingHeader = 12;
                scenarioEra = ERA_MODERN;
            }

            if (armyPlaying != _armyPlayingIndexOriginal) {
                // not our turn
                return false;
            }

            if(scenarioEra != _scenarioEra) {
                return false;
            }

            string line;
            string oob_filename = "", map_filename = "", pdt_filename = "";
            for (int i = 0; i < remainingHeader; i++) {
                line = ReadString(scenario);
                if (line.IndexOf(".oob") != -1) {
                    oob_filename = line;
                } else if (line.IndexOf(".map") != -1) {
                    map_filename = line;
                } else if (line.IndexOf(".pdt") != -1) {
                    pdt_filename = line;
                }
            }

            if(
                maxTurns        != _maxTurns    || 
                oob_filename    != _oobFilename ||
                map_filename    != _mapFilename ||
                pdt_filename    != _pdtFilename
            ) {
                return false;
            }

            if(currentTurn > _currentTurn && armyPlaying == _armyPlayingIndexOriginal && phase == 0) {
                return true;
            }

            return false;
        }

        public List<Dictionary<string, object>> GetAllUnits() {
            return _unitData.Values.ToList();
        }

        public void DetermineUnitPresence(ScenarioReader scenario) {
            _unitPresent = new Dictionary<string, bool>();

            // Store current position of all leader units
            string line = ReadString(scenario);
            string[] items;
            while (line != null) {
                if(line.Contains("PEM Middle")) {
                    break;
                }

                items = line.Split();
                if (items.Length >= 2) {
                    if (items[0] == "1") { // unit placement
                        string code = items[1];
                        if (!_unitPresent.ContainsKey(code)) {
                            _unitPresent.Add(code, true);
                        }
                    }
                }

                line = ReadString(scenario);
            }
        }

        private TreeNode FindTreeNodeByText(TreeNode root, string text) {
            if (root.Text.Equals(text)) return root;

            foreach (TreeNode node in root.Nodes) {
                if (node.Text.Equals(text)) return node;
                TreeNode next = FindTreeNodeByText(node, text);
                if (next != null) return next;
            }
            return null;
        }

        private TreeNode SearchTreeViewByText(System.Windows.Forms.TreeView tree, string text) {
            foreach(TreeNode node in tree.Nodes) {
                TreeNode next = FindTreeNodeByText(node, text);
                if (next != null) return next;  
            }

            return null;
        }

        public bool IsUnitFriendly(Dictionary<string, object> unit) {
            string ourNation = GetNation();
            string unitNation = (string)unit["nation"];
            return CoalitionEraTracker.AreNationsAllied(
                _year, 
                ourNation, 
                unitNation
            );
        }

        public void UpdateUnitLocations(ScenarioReader scenario) {
            // Store current position of all leader units
            Dictionary<string, bool> unitsInScenario = new Dictionary<string, bool>();
            foreach(string code in _unitPresent.Keys.ToList()) {
                unitsInScenario.Add(code, false);
            }

            // Update locations of units in scenario
            List<Dictionary<string, object>> units = _unitData.Values.ToList();
            string line = ReadString(scenario);
            string[] items;
            while (line != null) {
                if(line.Contains("PEM Middle")) {
                    break;
                }

                items = line.Split();
                if (items.Length >= 2) {
                    if (items[0] == "1") { // unit placement
                        string code = items[1];
                        for (int i = 0; i < units.Count; i++) {
                            Dictionary<string, object> unit = units[i];
                            if (code == (string)unit["code"]) {
                                string node_name = (string)unit["node_name"];
                                unit = _unitData[node_name];

                                int location_x = int.Parse(items[items.Length - 2]);
                                int location_y = int.Parse(items[items.Length - 1]);
                                unit["location"] = new Location(location_x, location_y);

                                if (_unitPresent.ContainsKey(code)) {
                                    _unitPresent[code] = true;

                                    if (unitsInScenario.ContainsKey(code)) {
                                        unitsInScenario.Remove(code);
                                    }
                                } else {
                                    _unitPresent.Add(code, true);
                                }
                            }
                        }
                    }
                }

                line = ReadString(scenario);
            }

            // For any units that are no longer in the scenario, mark them absent
            // and set location off-map
            foreach(string code in unitsInScenario.Keys.ToList()) {
                _unitPresent[code] = false;
                foreach (Dictionary<string, object> unit in units) {
                    if ((string)unit["code"] == code) {
                        unit["location"] = new Location();
                    }
                }
            }
        }

        public void UpdateHQHierarchy() {
            // Color-code present/absent units in the treeview
            if (_tvRecip.Nodes.Count > 0) {
                // Update locations of units in scenario
                List<Dictionary<string, object>> units = _unitData.Values.ToList();

                foreach (Dictionary<string, object> unit in units) {
                    if (IsUnitFriendly(unit)) {
                        string node_name = (string)unit["node_name"];
                        Location loc = (Location)unit["location"];

                        TreeNode recipResult = SearchTreeViewByText(_tvRecip, node_name);
                        TreeNode senderResult = SearchTreeViewByText(_tvSender, node_name);
                        if (recipResult != null && senderResult != null) {
                            if (loc.IsPresent()) {
                                TreeViewUtil.treeNodeSetForeColor(
                                    _tvRecip, recipResult, System.Drawing.Color.Black
                                );
                                TreeViewUtil.treeNodeSetForeColor(
                                    _tvSender, senderResult, System.Drawing.Color.Black
                                );
                            } else {
                                TreeViewUtil.treeNodeSetForeColor(
                                    _tvRecip, recipResult, System.Drawing.Color.Gray
                                );
                                TreeViewUtil.treeNodeSetForeColor(
                                    _tvSender, senderResult, System.Drawing.Color.Gray
                                );
                            }
                        }
                    }
                }
            }
        }

        public bool InEnemyZOC(Location loc) {
            List<Dictionary<string, object>> units = _unitData.Values.ToList();
            foreach(Dictionary<string, object> unit in units) {
                bool isFriendly = IsUnitFriendly(unit);
                Location enemy_loc = (Location)unit["location"];
                if(!isFriendly && enemy_loc.IsPresent()) {
                    if(loc.DistanceTo(enemy_loc) <= 1) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReadScenario(string filename, string wdsPath) {
            string[] all_lines = File.ReadAllLines(
                filename,
                Encoding.GetEncoding("ISO-8859-1")
            );
            ScenarioReader scenario = new ScenarioReader(all_lines);

            ParseScenarioHeader(scenario);

            string oobPath;
            if (_scenarioEra == ERA_PREMODERN) {
                oobPath = wdsPath + @"\OOBs";
            } else {
                oobPath = wdsPath + @"\Data";
            }

            int filePosition = scenario.Position();
            DetermineUnitPresence(scenario);
            scenario.SetPosition(filePosition);

            if (!ReadOOB(oobPath + @"\" + _oobFilename)) {
                _loadedCorrectly = false;
                return;
            }

            string mapPath = wdsPath + @"\Maps\";
            if(!ReadMap(mapPath, _mapFilename)) {
                _loadedCorrectly = false;
                return;
            }

            UpdateUnitLocations(scenario);

            _loadedCorrectly = true;
        }

        public bool PopulateUI() {
            if(!PopulateTreeWithArmies()) {
                MessageBox.Show(
                    "Can't find a valid army!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
            UpdateHQHierarchy();

            return true;
        }

        private string GetAvailableUnitName(string uname) {
            int dupUnitNames = 0;
            string nodeName = uname;
            string origNodeName = uname;
            while (_unitData.ContainsKey(nodeName)) {
                dupUnitNames++;
                nodeName = origNodeName + " (" + dupUnitNames + ")";
            }

            return nodeName;
        }

        private Dictionary<string, object> AddUnit(
            string code,
            string nodeName,
            string parentNode,
            string messageName,
            Location location,
            string nation,
            string uType,
            string name
        ) {
            Dictionary<string, object> unitDict = new Dictionary<string, object> {
                { "code",           code        },
                { "node_name",      nodeName    },
                { "parent_node",    parentNode  },
                { "message_name",   messageName },
                { "location",       location    },
                { "nation",         nation      },
                { "type",           uType       },
                { "name",           name        },
            };
            _unitData.Add(nodeName, unitDict);

            return unitDict;
        }

        private void AppendUnit(
            Dictionary<string, object> unit,
            string code,
            string nodeName,
            string parentNode,
            string messageName,
            Location location,
            string nation,
            string uType
        ) {
            unit["code"] = code;
            unit["node_name"] = nodeName;
            unit["parent_node"] = parentNode;
            unit["message_name"] = messageName;
            unit["location"] = location;
            unit["nation"] = nation;
            unit["type"] = uType;

            if (nodeName != "") {
                _unitData[nodeName] = unit;
            }
        }

        private void PopulateTreeNode(
            TreeNode headNode, 
            List<Dictionary<string, object>> units, 
            string currentCode
        ) {
            if (units.Count == 0) { return; }

            // find the leader on the ground, if he's in the scenario
            // otherwise use the default commander
            bool leaderFound = false;
            Dictionary<string, object> leaderUnit = null;
            foreach(Dictionary<string, object> unit in units) {
                if ((string)unit["type"] == "Leader") {
                    if (
                        unit.ContainsKey("node_name") &&
                        unit.ContainsKey("code") &&
                        unit.ContainsKey("message_name")
                    ) {
                        leaderFound = true;
                        leaderUnit = unit;
                        break;
                    }
                }
            }

            if (!leaderFound) {
                // no leaders at all?? find a unit
                foreach (Dictionary<string, object> unit in units) {
                    if ((string)unit["type"] == "Unit") {
                        if (
                            unit.ContainsKey("node_name") &&
                            unit.ContainsKey("code") &&
                            unit.ContainsKey("message_name")
                        ) {
                            leaderFound = true;
                            leaderUnit = unit;
                            break;
                        }
                    }
                }
            }

            if(!leaderFound) {
                return;
            }

            // show it on the list
            string nodeName = (string)leaderUnit["node_name"];
            headNode.Nodes.Add(nodeName);
            headNode.Nodes[headNode.Nodes.Count - 1].ForeColor = System.Drawing.Color.Gray;

            TreeNode subNode = headNode.Nodes[headNode.Nodes.Count - 1];

            for (int i = 0; i < units.Count; i++) {
                Dictionary<string, object> unit = units[i];

                string subunitType = (string)unit["type"];
                string newCode = currentCode + "." + (i + 1).ToString();

                if (subunitType != "Leader" && subunitType != "Unit") {
                    List<Dictionary<string, object>> formUnits = (
                        List<Dictionary<string, object>>
                    )unit["units"];
                    PopulateTreeNode(subNode, formUnits, newCode);
                }
            }
        }

        private bool PopulateTreeWithArmies() {
            string friendlyNation;
            if (_armyPlayingIndex < _nations.Count) {
                friendlyNation = _nations[_armyPlayingIndex];
            } else if (_armyPlayingIndex < _armies.Count) {
                Dictionary<string, object> friendlyArmy = _armies[_armyPlayingIndex];
                friendlyNation = (string)friendlyArmy["nation"];
            } else {
                // TODO show error message
                return false;
            }

            _tvRecip.BeginUpdate();
            _tvRecip.Nodes.Clear();
            for (int i = 0; i < _armies.Count; i++) {
                Dictionary<string, object> army = _armies[i];
                string armyNation = (string)army["nation"];

                string codePrefix = (i + 1).ToString();

                bool friendly = CoalitionEraTracker.AreNationsAllied(
                    _year, 
                    friendlyNation, 
                    armyNation
                );

                List<Dictionary<string, object>> units = (
                    List<Dictionary<string, object>>
                )army["units"];

                if (units.Count() == 0 || !friendly) {
                    continue;
                }

                // find the leader on the ground, if he's in the scenario
                // otherwise use the default commander
                bool leaderFound = false;
                Dictionary<string, object> leaderUnit = null;
                for(int j = 0; j < units.Count(); j++) {
                    Dictionary<string, object> test_unit = units[j];
                    if ((string)test_unit["type"] == "Leader") {
                        if (
                            test_unit.ContainsKey("node_name") && 
                            test_unit.ContainsKey("code") && 
                            test_unit.ContainsKey("message_name")
                        ) {
                            leaderFound = true;
                            leaderUnit = test_unit;
                            break;
                        }
                    }
                }

                if (!leaderFound) {
                    // no leaders at all?? find a unit
                    for (int j = 0; j < units.Count(); j++) {
                        Dictionary<string, object> test_unit = units[j];
                        if ((string)test_unit["type"] == "Unit") {
                            if (
                            test_unit.ContainsKey("node_name") &&
                            test_unit.ContainsKey("code") &&
                            test_unit.ContainsKey("message_name")
                            ) {
                                leaderFound = true;
                                leaderUnit = test_unit;
                                break;
                            }
                        }
                    }
                }

                if(!leaderFound) {
                    continue;
                }

                string nodeName = (string)leaderUnit["node_name"];

                // Add to the tree
                _tvRecip.Nodes.Add(nodeName);
                _tvRecip.Nodes[_tvRecip.Nodes.Count - 1].ForeColor = System.Drawing.Color.Gray;

                TreeNode subNode = _tvRecip.Nodes[_tvRecip.Nodes.Count - 1];

                for (int j = 0; j < units.Count; j++) {
                    Dictionary<string, object> unit = units[j];
                    string unitType = (string)unit["type"];

                    if (unitType != "Leader" && unitType != "Unit") {
                        List<Dictionary<string, object>> formation = (
                            List<Dictionary<string, object>>
                        )unit["units"];
                        
                        string newCode = codePrefix + "." + (j + 1).ToString();

                        PopulateTreeNode(
                            subNode,
                            formation,
                            newCode
                        );
                    }
                }
            }
            _tvRecip.EndUpdate();
            _tvRecip.ExpandAll();

            // Copy same nodes to sender list
            _tvSender.BeginUpdate();
            _tvSender.Nodes.Clear();
            CopyNodes(_tvSender.Nodes, _tvRecip.Nodes);
            _tvSender.EndUpdate();
            _tvSender.ExpandAll();

            return true;
        }

        private void CopyNodes(TreeNodeCollection nodes_to, TreeNodeCollection nodes_from) {
            foreach (TreeNode node in nodes_from) {
                nodes_to.Add(node.Text);
                nodes_to[nodes_to.Count - 1].ForeColor = node.ForeColor;

                TreeNodeCollection subnodes_to = nodes_to[nodes_to.Count - 1].Nodes;
                CopyNodes(subnodes_to, node.Nodes);
            }
        }

        private void PopulateArmies() {
            for (int i = 0; i < _armies.Count; i++)
            {
                Dictionary<string, object> army = _armies[i];
                string armyNation = (string)army["nation"];
                string armyName = (string)army["name"];

                string codePrefix = (i + 1).ToString();

                List<Dictionary<string, object>> units = (
                    List<Dictionary<string, object>>
                )army["units"];

                if(units.Count() == 0) {
                    continue;
                }

                // find the leader on the ground, if he's in the scenario
                // otherwise use the default commander
                bool leaderFound = false;
                int leaderIndex = 0;
                Dictionary<string, object> leaderUnit = null;
                while (!leaderFound && leaderIndex < units.Count()) {
                    Dictionary<string, object> test_unit = units[leaderIndex];
                    if ((string)test_unit["type"] == "Leader") {
                        string leaderCode = codePrefix + "." + (leaderIndex + 1);
                        if (_unitPresent.ContainsKey(leaderCode)) {
                            leaderFound = true;
                            leaderUnit = test_unit;
                        } else {
                            leaderIndex++;
                        }
                    } else {
                        // have run out of leaders
                        break;
                    }
                }

                string nodeName, messageName;
                // no leaders present? find A leader
                if (!leaderFound) {
                    for (int j = 0; j < units.Count(); j++) {
                        Dictionary<string, object> test_unit = units[j];
                        if ((string)test_unit["type"] == "Leader") {
                            leaderIndex = j;
                            leaderUnit = test_unit;
                            leaderFound = true;
                            break;
                        }
                    }
                }

                if (!leaderFound) {
                    // no leaders at all?? find a unit
                    for (int j = 0; j < units.Count(); j++) {
                        Dictionary<string, object> test_unit = units[j];
                        if ((string)test_unit["type"] == "Unit") {
                            leaderIndex = j;
                            leaderFound = true;
                            break;
                        }
                    }
                    nodeName = GetAvailableUnitName(armyName);
                    messageName = armyName;

                    if (!leaderFound) {
                        leaderIndex = -1;
                        // Add a dummy unit
                        leaderUnit = AddUnit(
                            codePrefix + ".0",
                            nodeName,
                            "",
                            messageName,
                            new Location(),
                            armyNation,
                            "Leader",
                            "Anonymous"
                        );
                        units.Add(leaderUnit);
                    }
                } else {
                    // Add the leader
                    string leaderName = (string)leaderUnit["name"];
                    nodeName = GetAvailableUnitName(armyName + " (" + leaderName + ")");
                    messageName = leaderName + ", " + armyName;

                    // Index node to unit data
                    AppendUnit(
                        leaderUnit,
                        codePrefix + "." + (leaderIndex + 1),
                        nodeName,
                        "",
                        messageName,
                        new Location(),
                        armyNation,
                        "Leader"
                    );
                }

                for (int j = 0; j < units.Count; j++) {
                    Dictionary<string, object> unit = units[j];
                    string unitType = (string)unit["type"];

                    string newCode = codePrefix + "." + (j + 1).ToString();
                    string unitName = (string)unit["name"];

                    if (unitType != "Leader" && unitType != "Unit") {
                        List<Dictionary<string, object>> formation = (
                            List<Dictionary<string, object>>
                        ) unit["units"];
                        PopulateFormations(
                            armyNation,
                            unitName, 
                            armyName, 
                            nodeName, 
                            formation, 
                            newCode
                        );
                    } else if(unitType == "Unit") {
                        string unitId;
                        if (j == leaderIndex) {
                            // if we're using a unit as an HQ to receive dispatches,
                            // in the absence of an official leader...
                            unitId = nodeName;
                            unitName = armyName;
                        } else {
                            unitId = unitName + " " + newCode;
                        }

                        AppendUnit(
                            unit,
                            newCode,
                            unitId,
                            nodeName,
                            unitName,
                            new Location(),
                            armyNation,
                            unitType
                        );
                    }
                }
            }
        }

        public void PopulateFormations(
            string armyNation,
            string unitName,
            string parentUnitname,
            string parentNodename,
            List<Dictionary<string, object>> formation,
            string currentCode
        ) {
            if (formation.Count == 0) { return; }

            // find the leader on the ground, if he's in the scenario
            // otherwise use the default commander
            bool leaderFound = false;
            int leaderIndex = 0;
            Dictionary<string, object> leaderUnit = null;
            while (!leaderFound && leaderIndex < formation.Count()) {
                Dictionary<string, object> test_unit = formation[leaderIndex];
                if ((string)test_unit["type"] == "Leader") {
                    string leaderCode = currentCode + "." + (leaderIndex + 1);
                    if (_unitPresent.ContainsKey(leaderCode)) {
                        leaderFound = true;
                        leaderUnit = test_unit;
                    } else {
                        leaderIndex++;
                    }
                } else {
                    // have run out of leaders
                    break;
                }
            }

            string nodeName, messageName;

            // no leaders present? find A leader
            if (!leaderFound) {
                for (int j = 0; j < formation.Count(); j++) {
                    Dictionary<string, object> test_unit = formation[j];
                    if ((string)test_unit["type"] == "Leader") {
                        leaderIndex = j;
                        leaderUnit = test_unit;
                        leaderFound = true;
                        break;
                    }
                }
            }

            if (!leaderFound) {
                // no leaders present, find a unit instead
                for (int i = 0; i < formation.Count(); i++) {
                    Dictionary<string, object> test_unit = formation[i];
                    if ((string)test_unit["type"] == "Unit") {
                        leaderIndex = i;
                        leaderFound = true;
                        break;
                    }
                }
                nodeName = GetAvailableUnitName(unitName);
                messageName = unitName + ", " + parentUnitname;

                if (!leaderFound) {
                    leaderIndex = -1;
                    // Add a dummy unit
                    leaderUnit = AddUnit(
                        currentCode + ".0",
                        nodeName,
                        parentNodename,
                        messageName,
                        new Location(),
                        armyNation,
                        "Leader",
                        "Anonymous"
                    );
                    formation.Add(leaderUnit);
                }
            } else {
                string leaderName = (string)leaderUnit["name"];
                nodeName = GetAvailableUnitName(unitName + " (" + leaderName + ")");
                messageName = leaderName + ", " + unitName;

                // Index node to unit data
                AppendUnit(
                    leaderUnit,
                    currentCode + "." + (leaderIndex + 1),
                    nodeName,
                    parentNodename,
                    messageName,
                    new Location(),
                    armyNation,
                    "Leader"
                );
            }

            for (int i = 0; i < formation.Count; i++) {
                Dictionary<string, object> unit = formation[i];

                string subunitType = (string)unit["type"];
                string newCode = currentCode + "." + (i + 1).ToString();
                string subunitName = (string)unit["name"];

                if (subunitType != "Leader" && subunitType != "Unit") {
                    List<Dictionary<string, object>> formUnits = (List<Dictionary<string, object>>)unit["units"];
                    PopulateFormations(
                        armyNation,
                        subunitName,
                        unitName,
                        nodeName,
                        formUnits,
                        newCode
                    );
                } else if (subunitType == "Unit") {
                    string unitId;
                    if (i == leaderIndex) {
                        // Using it as HQ proxy for receiving dispatches
                        unitId = nodeName;
                        subunitName = unitName;
                    } else {
                        unitId = subunitName + " " + newCode;
                    }

                    AppendUnit(
                        unit,
                        newCode,
                        unitId,
                        parentNodename,
                        subunitName,
                        new Location(),
                        armyNation,
                        subunitType
                    );
                }
            }
        }

        public void LoadScenario(string battlePath, System.Windows.Forms.TreeView tvRecip, System.Windows.Forms.TreeView tvSender) {
            _tvRecip = tvRecip;
            _tvSender = tvSender;

            string savesPath = Path.GetDirectoryName(battlePath);

            // Move one directory up to get home game folder
            string wdsPath = Path.GetFullPath(
                Path.Combine(savesPath, "..")
            );

            // Initialize unit data from scratch
            _unitData.Clear();
            ReadScenario(battlePath, wdsPath);

            _filename = battlePath;
            _shortFilename = Path.GetFileName(battlePath);
        }

        public static ScenarioReader WaitForFile(string filename) {
            int WAIT_TIME_SECONDS = 30;  // seconds
            int CHECK_TIME_MILLIS = 250; // ms

            bool available = false;
            string[] all_lines = null;
            ScenarioReader scenario = null;

            int iterations = WAIT_TIME_SECONDS * 1000 / CHECK_TIME_MILLIS;
            for (int i = 0; i < iterations; i++) {
                try {
                    all_lines = File.ReadAllLines(
                        filename, 
                        Encoding.GetEncoding("ISO-8859-1")
                    );

                    available = true;
                } catch (IOException) {
                    available = false;
                    System.Threading.Thread.Sleep(CHECK_TIME_MILLIS);
                }

                if (available) {
                    scenario = new ScenarioReader(all_lines);
                    break; 
                }
            }

            return scenario;
        }

        public void Refresh() {
            ScenarioReader scenario = WaitForFile(_filename);

            if (scenario == null) {
                throw new Exception("File in use!");
            }

            if (CheckScenarioHeader(scenario) == false) {
                return;
            }

            scenario = WaitForFile(_filename);

            if (scenario == null) {
                throw new Exception("File in use!");
            }

            ParseScenarioHeader(scenario);
            UpdateUnitLocations(scenario);
            UpdateHQHierarchy();

            _hasUpdated = true;
        }

        public Location GetUnitLocation(string unitname) {
            Dictionary<string, object> _unitStats = _unitData[unitname];

            if (_unitStats != null) {
                return (Location)_unitStats["location"];
            } else {
                throw new Exception("Can't find " + unitname + "!");
            }
        }

        public void BuildRecipientChain(
            ref List<string> recipientChain,
            Dictionary<string, object> sender,
            Dictionary<string, object> recipient
        ) {
            List<string> buildHierarchy(Dictionary<string, object> unit) {
                List<string> hierarchy = new List<string>();
                string unitParentNode = (string)unit["parent_node"];
                while(unitParentNode != "") {
                    hierarchy.Add(unitParentNode);
                    Dictionary<string, object> parentUnit = GetUnitDataByNodeName(unitParentNode);
                    unitParentNode = (string)parentUnit["parent_node"];
                }

                return hierarchy;
            }

            recipientChain.Clear();

            string senderNodeName = (string)sender["node_name"];
            string recipNodeName = (string)recipient["node_name"];
            List<string> senderHierarchy = buildHierarchy(sender);
            List<string> recipHierarchy = buildHierarchy(recipient);

            // Find shortest path through hierarchy between sender and receiver
            
            // First, see if recipient is in sender's direct hierarchy
            for(int i = 0; i < senderHierarchy.Count; i++) {
                if (senderHierarchy[i] == recipNodeName) {
                    recipientChain = senderHierarchy.GetRange(0, i);
                    return;
                }
            }

            // If not, see if sender is in recipient's direct hierarchy
            for (int i = 0; i < recipHierarchy.Count; i++) {
                if (recipHierarchy[i] == senderNodeName) {
                    recipientChain = recipHierarchy.GetRange(0, i);
                    recipientChain.Reverse();
                    return;
                }
            }

            // If not, try to find a common parent in the two hierarchies, then
            // link them
            for (int i = 0; i < senderHierarchy.Count; i++) {
                string senderNode = senderHierarchy[i];
                for (int j = 0; j < recipHierarchy.Count; j++) {
                    string recipNode = recipHierarchy[j];
                    if(senderNode == recipNode) {
                        List<string> reverseRecip = recipHierarchy.GetRange(0, j);
                        reverseRecip.Reverse();
                        recipientChain = senderHierarchy.GetRange(0, i + 1);
                        recipientChain.AddRange(reverseRecip);
                        return;
                    }
                }
            }

            // If still no match, then message has to pass
            // through both hierarchies in full
            recipHierarchy.Reverse();
            senderHierarchy.AddRange(recipHierarchy);
            recipientChain = senderHierarchy;
        }

        public int FindDistance(
            List<string> recipientChain, 
            Dictionary<string, object> sender,
            Dictionary<string, object> recipient
        ) {
            Location recipLoc = (Location)recipient["location"];
            Location senderLoc = (Location)sender["location"];

            int distance = -1;
            Location a = senderLoc;
            if (senderLoc.IsPresent() && recipLoc.IsPresent()) {
                Location b;
                distance = 0;
                if (recipientChain.Count > 0) {
                    foreach (string recipientName in recipientChain) {
                        Dictionary<string, object> unitInfo = GetUnitDataByNodeName(recipientName);
                        b = (Location)unitInfo["location"];
                        if (b.IsPresent()) {
                            distance += a.DistanceTo(b);
                            a = b;
                        }
                    }
                }
                b = (Location)recipient["location"];
                distance += a.DistanceTo(b);
            }

            return distance;
        }
    }
}
