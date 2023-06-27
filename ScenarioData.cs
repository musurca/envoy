﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WDS_Dispatches
{
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
        private Dictionary<string, bool> _unitPresent;
        private string  _scenarioName;
        private int     _armyPlaying;
        private int     _currentTurn;
        private string  _currentDay;
        private string  _currentTime;
        private int     _maxTurns;
        private string  _filename;
        private string  _oobFilename;
        private string  _shortFilename;
        private string  _mapFilename;
        private string  _pdtFilename;
        private bool    _hasUpdated;
        private int     _scenarioEra;
        private bool    _isPBEM;

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

        public bool IsPBEM() { return _isPBEM; }

        public int GetCurrentTurn() { return _currentTurn; }

        public int GetMaxTurns() { return _maxTurns; }

        public string GetFilenameFullPath() { return _filename; }

        public string GetFilename() { return _shortFilename;  }

        public string GetOOBFilename() { return _oobFilename; }

        public string GetPDTFilename() { return _pdtFilename; }

        public string GetMapFilename() { return _mapFilename; }

        public ScenarioData() {
            _armies = null;
            _unitData = null;

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
            return _unitData[node_name];
        }

        public List<string> GetObjectives() { return _objectives; }

        private static string GetMonthNameByIndex(int index)
        {
            if (index > 0 && index < 13) {
                return ScenarioData.Months[index-1];
            }

            return "Invalid";
        }

        public static int ReadNum(ScenarioReader f) {
            string line = f.ReadLine();
            if (line != null) {
                return int.Parse(line.Trim());
            }
            return -1;
        }

        public static string ReadString(ScenarioReader f) {
            string line = f.ReadLine();
            if (line != null) {
                return line.Trim();
            }
            return "";
        }

        public static Dictionary<string, object> ReadUnit(ScenarioReader f, string line) {
            string[] items = line.Split();
            string unitType = items[0].ToLower();

            Dictionary<string, string> abbrevToType = new Dictionary<string, string> {
                {"u", "Armed Unit" },
                {"s", "Supply" },
                {"g", "Ship" }
            };

            string unitName = "";
            for (int i = 12; i < items.Length; i++) {
                unitName += items[i] + " ";
            }
            unitName = unitName.Trim();

            return new Dictionary<string, object> {
                { "name", unitName },
                { "type", "Unit" },
                { "subtype", abbrevToType[unitType] }
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
                } catch(Exception e) {
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
                char lineType = char.ToLower(readLine[0]);
                if (lineType == 'l') {
                    units.Add(
                        ReadLeader(f, readLine)
                    );
                } else if (lineType == 'u' || lineType == 's' || lineType == 'g') {
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

            return new Dictionary<string, object> {
                { "name", formName },
                { "type", formKeys[formType] },
                { "units", units }
            };
        }

        public static Dictionary<string, object> ReadArmy(ScenarioReader f, string line) {
            string[] items = line.Split();
            if(items[1] != "A") {
                // TODO: might be a supply unit
                return null;
            }
            string nation = items[0];
            string armyName = "";
            for(int i = 2; i < items.Length; i++) {
                armyName += items[i] + " "; 
            }
            armyName = armyName.Trim();

            List<Dictionary<string, object>> units = new List<Dictionary<string, object>>();

            string readLine = ReadString(f);
            if (readLine.ToLower() != "begin") {
                throw new Exception("Expected 'begin'");
            }

            readLine = ReadString(f);
            while (readLine.ToLower() != "end") {
                char lineType = char.ToLower(readLine[0]);
                if (lineType == 'l') {
                    units.Add(
                        ReadLeader(f, readLine)
                    );
                }
                else if (lineType == 'u' || lineType == 's' || lineType == 'g') { // Unit or supply
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

            return new Dictionary<string, object> {
                { "name", armyName },
                { "type", "Army" },
                { "nation", nation },
                { "units", units }
            };
        }

        public void ReadOOB(string filename, TreeView tvRecip, TreeView tvSender) {
            _armies = new List<Dictionary<string, object>>();

            string[] all_lines = File.ReadAllLines(
                filename,
                Encoding.GetEncoding("ISO-8859-1")
            );
            
            ScenarioReader oob = new ScenarioReader(all_lines);

            int oobNum = ReadNum(oob);
            if (_scenarioEra == ERA_MODERN) {
                int oobId = ReadNum(oob);
            }
            
            string line = ReadString(oob);
            while (line != "") {
                Dictionary<string, object> army = ReadArmy(oob, line);
                if (army != null) {
                    _armies.Add(army);
                }
                line = ReadString(oob);
            }

            PopulateArmies(tvRecip, tvSender);
        }

        public void ParseScenarioHeader(ScenarioReader scenario) {
            int fileVersion;
            string first_line = ReadString(scenario).Trim();

            if(first_line.Contains("PEM Header")) {
                _isPBEM = true;
                ReadString(scenario); // Not sure what this is, but we don't use it
                fileVersion = ReadNum(scenario);
            } else {
                _isPBEM = false;
                fileVersion = int.Parse(first_line);
            }

            _scenarioName = ReadString(scenario).Trim();

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
                _armyPlaying = int.Parse(items[5]);
                phase = int.Parse(items[6]);
                _currentTurn = int.Parse(items[7]);
                _maxTurns = int.Parse(items[8]);

                remainingHeader = 9;
                _scenarioEra = ERA_PREMODERN;
            } else {
                month = int.Parse(items[1]);
                day = int.Parse(items[2]);
                hour = int.Parse(items[3]);
                _armyPlaying = int.Parse(items[4]);
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

        public void ReadMap(string basepath, string filename) {
            string[] all_lines = File.ReadAllLines(
                basepath + filename,
                Encoding.GetEncoding("ISO-8859-1")
            );

            ScenarioReader mapfile = new ScenarioReader(all_lines);

            int x = -1, 
                y = -1,
                width = -1,
                height = -1;

            int map_version = ReadNum(mapfile);
            
            if(map_version == 0) {
                // Submap
                string new_mapfile = ReadString(mapfile);
                string[] dims = ReadString(mapfile).Split();
                x = int.Parse(dims[0]);
                y = int.Parse(dims[1]);
                width = int.Parse(dims[2]);
                height = int.Parse(dims[3]);

                all_lines = File.ReadAllLines(
                    basepath + new_mapfile,
                    Encoding.GetEncoding("ISO-8859-1")
                );

                mapfile = new ScenarioReader(all_lines);
            } else {
                mapfile.Reset();
            }

            ReadScenarioMap(mapfile, x, y, width, height);
        }

        public void ReadScenarioMap(ScenarioReader map, int x, int y, int w, int h) {
            int fileVersion = ReadNum(map);
            string[] dimensions = ReadString(map).Split();
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);
            string map_metadata = ReadString(map);

            // Now read terrain and height data, then ignore it
            for (int i = 0; i < 2*height; i++) {
                ReadString(map);
            }

            int layers = fileVersion == 1 ? 10 : 12;

            // Read layer data, then ignore it
            for (int j = 0; j < layers; j++) {
                if (ReadNum(map) == 1 ) {
                    for (int i = 0; i < height; i++) {
                        ReadString(map);
                    }
                }
            }

            _objectives = new List<string>();
            string objective_str = ReadString(map);
            while(objective_str != "") {
                string[] items = objective_str.Split();
                if (items.Count() > 5) {
                    string objective_name = items[5];
                    for (int i = 6; i < items.Count(); i++) {
                        objective_name = objective_name + " " + items[i];
                    }

                    float obj_x = float.Parse(items[0]);
                    float obj_y = float.Parse(items[1]);

                    if (x != -1) {
                        if (obj_x < x || obj_x > (x + w) || obj_y < y || obj_y > (y + h)) {
                            objective_str = ReadString(map);
                            continue;
                        }
                    }

                    // No duplicate objective names
                    if (!_objectives.Contains(objective_name)) {
                        _objectives.Add(objective_name);
                    }
                }

                objective_str = ReadString(map);
            }
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
                throw new Exception("Battle file corrupted!");
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

            if (armyPlaying != _armyPlaying) {
                // not our turn
                return false;
            }

            if(scenarioEra != _scenarioEra) {
                throw new Exception("Battle file corrupted!");
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
                throw new Exception("Battle file corrupted!");
            }

            if(currentTurn > _currentTurn && armyPlaying == _armyPlaying && phase == 0) {
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
            while (line != "" && !line.Contains("PEM Middle")) {
                items = line.Split();
                if (items[0] == "1") { // unit placement
                    string code = items[1];
                    if (!_unitPresent.ContainsKey(code)) {
                        _unitPresent.Add(code, true);
                    }
                }

                line = ReadString(scenario);
            }
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
            while (line != "" && !line.Contains("PEM Middle")) {
                items = line.Split();
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
                                unitsInScenario.Remove(code);
                            } else {
                                _unitPresent.Add(code, true);
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
                        break;
                    }
                }
            }
        }

        public bool InEnemyZOC(Location loc) {
            List<Dictionary<string, object>> units = _unitData.Values.ToList();
            foreach(Dictionary<string, object> unit in units) {
                bool isFriendly = (bool)unit["friendly"];
                Location enemy_loc = (Location)unit["location"];
                if(!isFriendly && enemy_loc.IsPresent()) {
                    if(loc.DistanceTo(enemy_loc) <= 1) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReadScenario(string filename, string wdsPath, TreeView tvRecip, TreeView tvSender) {
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

            ReadOOB(oobPath + @"\" + _oobFilename, tvRecip, tvSender);

            string mapPath = wdsPath + @"\Maps\";
            ReadMap(mapPath, _mapFilename);

            UpdateUnitLocations(scenario);
        }

        public void PopulateFormations(
            string unitName,
            string parentUnitname,
            string parentNodename,
            TreeNode headNode, 
            List<Dictionary<string, object>> formation,
            string currentCode,
            bool friendly
        ) {
            if(formation.Count == 0) { return; }

            // find the leader on the ground, if he's in the scenario
            // otherwise use the default commander
            bool leaderFound = false;
            int leaderIndex = 0;
            Dictionary<string, object> leaderUnit = null;
            do {
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

            } while (!leaderFound && leaderIndex < formation.Count());

            if (!leaderFound) {
                leaderIndex = 0;
                leaderUnit = formation[0];
            }

            //Dictionary<string, object> firstUnit = formation[0];
            string uType = (string)leaderUnit["type"];
            string nodeName, messageName;
            if (uType == "Leader") {
                string leaderName = (string)leaderUnit["name"];
                nodeName = unitName + " (" + leaderName + ")";
                messageName = leaderName + ", " + unitName;
            } else {
                nodeName = unitName;
                messageName = unitName + ", " + parentUnitname;
            }
            int dupUnitNames = 0;
            string origNodeName = nodeName;
            while (_unitData.ContainsKey(nodeName)) {
                dupUnitNames++;
                // Possible another army has an identical unit name
                nodeName = origNodeName + " (" + dupUnitNames + ")";
            }
            if (friendly) {
                // show it on the list
                headNode.Nodes.Add(nodeName);
            }

            // Index node to unit data
            Dictionary<string, object> unitDict = new Dictionary<string, object> {
                { "code",           currentCode + "." + (leaderIndex+1) },
                { "node_name",      nodeName },
                { "parent_node",    parentNodename },
                { "message_name",   messageName },
                { "location",       new Location() },
                { "friendly",       friendly },
                { "type",           "Leader" }
            };
            _unitData.Add(nodeName, unitDict);

            TreeNode subNode;
            if (friendly) {
                subNode = headNode.Nodes[headNode.Nodes.Count - 1];
            } else {
                subNode = null;
            }

            for (int i = 1; i < formation.Count; i++) {
                Dictionary<string, object> unit = formation[i];

                string subunitType = (string)unit["type"];
                string newCode = currentCode + "." + (i + 1).ToString();
                string subunitName = (string)unit["name"];

                if (subunitType != "Leader" && subunitType != "Unit") {
                    List<Dictionary<string, object>> formUnits = (List<Dictionary<string, object>>)unit["units"];
                    PopulateFormations(
                        subunitName, 
                        unitName, 
                        nodeName, 
                        subNode, 
                        formUnits, 
                        newCode, 
                        friendly
                    );
                } else if (subunitType == "Unit") {
                    string unitId = subunitName + " " + newCode;
                    Dictionary<string, object> newUnitDict = new Dictionary<string, object> {
                        { "code",           newCode },
                        { "node_name",      unitId },
                        { "parent_node",    parentNodename },
                        { "message_name",   subunitName },
                        { "location",       new Location() },
                        { "friendly",       friendly },
                        { "type",           subunitType } 
                    };
                    _unitData.Add(unitId, newUnitDict);
                }
            }
        }

        private void PopulateArmies(TreeView tvRecip, TreeView tvSender) {
            // Populate the OOBs for recipient and sender
            tvRecip.BeginUpdate();
            tvRecip.Nodes.Clear();
            for (int i = 0; i < _armies.Count; i++)
            {
                bool friendly = (_armyPlaying == i);

                string codePrefix = (i + 1).ToString();

                Dictionary<string, object> army = _armies[i];
                string armyName = (string)army["name"];

                List<Dictionary<string, object>> units = (
                    List<Dictionary<string, object>>
                )army["units"];

                // find the leader on the ground, if he's in the scenario
                // otherwise use the default commander
                bool leaderFound = false;
                int leaderIndex = 0;
                Dictionary<string, object> leaderUnit = null;
                do {
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

                } while (!leaderFound && leaderIndex < units.Count());

                if(!leaderFound) {
                    leaderIndex = 0;
                    leaderUnit = units[0];
                }

                string leaderName = (string)leaderUnit["name"];
                string nodeName = armyName + " (" + leaderName + ")";

                int dupUnitNames = 0;
                string origNodeName = nodeName;
                while (_unitData.ContainsKey(nodeName)) {
                    dupUnitNames++;
                    nodeName = origNodeName + " (" + dupUnitNames + ")";
                }
                
                if (friendly) {
                    tvRecip.Nodes.Add(nodeName);
                }

                // Index node to unit data
                Dictionary<string, object> unitDict = new Dictionary<string, object> {
                    { "code",           codePrefix + "." + (leaderIndex+1)  },
                    { "node_name",      nodeName                            },
                    { "parent_node",    ""                                  },
                    { "message_name",   leaderName + ", " + armyName        },
                    { "location",       new Location()                      },
                    { "friendly",       friendly                            },
                    { "type",           "Leader"                            }
                };
                _unitData.Add(nodeName, unitDict);

                TreeNode armyNode;
                if (friendly) {
                    armyNode = tvRecip.Nodes[tvRecip.Nodes.Count - 1];
                } else {
                    armyNode = null;
                }

                for (int j = 1; j < units.Count; j++) {
                    Dictionary<string, object> unit = units[j];
                    string unitType = (string)unit["type"];

                    string newCode = codePrefix + "." + (j + 1).ToString();
                    string unitName = (string)unit["name"];

                    if (unitType != "Leader" && unitType != "Unit") {
                        List<Dictionary<string, object>> formation = (
                            List<Dictionary<string, object>>
                        ) unit["units"];
                        PopulateFormations(
                            unitName, 
                            armyName, 
                            nodeName, 
                            armyNode, 
                            formation, 
                            newCode, 
                            friendly
                        );
                    } else if(unitType == "Unit") {
                        string unitId = unitName + " " + newCode;
                        Dictionary<string, object> newUnitDict = new Dictionary<string, object> {
                            { "code",           newCode },
                            { "node_name",      unitId },
                            { "parent_node",    nodeName },
                            { "message_name",   unitName },
                            { "location",       new Location() },
                            { "friendly",       friendly },
                            { "type",           unitType }
                        };
                        _unitData.Add(unitId, newUnitDict);
                    }
                }
            }
            tvRecip.EndUpdate();
            tvRecip.ExpandAll();

            // Copy same nodes to sender list
            tvSender.BeginUpdate();
            tvSender.Nodes.Clear();
            CopyNodes(tvSender.Nodes, tvRecip.Nodes);
            tvSender.EndUpdate();
            tvSender.ExpandAll();
        }

        private void CopyNodes(TreeNodeCollection nodes_to, TreeNodeCollection nodes_from) {
            foreach (TreeNode node in nodes_from) {
                nodes_to.Add(node.Text);
                TreeNodeCollection subnodes_to = nodes_to[nodes_to.Count - 1].Nodes;
                CopyNodes(subnodes_to, node.Nodes);
            }
        }

        public void LoadScenario(string battlePath, TreeView tvRecip, TreeView tvSender) {
            string wdsPath = "";

            string savesPath = Path.GetDirectoryName(battlePath);
            wdsPath = Path.Combine(savesPath, "..");

            // Initialize unit data from scratch
            _unitData = new Dictionary<string, Dictionary<string, object>>();
            ReadScenario(battlePath, wdsPath, tvRecip, tvSender);

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
    }
}
