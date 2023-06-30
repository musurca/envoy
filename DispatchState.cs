using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WDS_Dispatches
{
    public class Dispatch {
        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("turn_sent")]
        public int TurnSent { get; set; }

        [JsonProperty("turns_in_transit")]
        public int TurnsInTransit { get; set; }

        [JsonProperty("cur_location")]
        public Location CurrentLocation { get; set; }

        [JsonProperty("recipient_chain")]
        public List<string> RecipientChain { get; set; }

        public Dispatch() {

        }

        public Dispatch(
            string sender, 
            string recipient, 
            string message, 
            Location starting_location,
            int turn_sent,
            List<string> recipientChain)
        {
            this.Sender = sender;
            this.Recipient = recipient;
            this.Message = message;
            this.CurrentLocation = starting_location;
            this.RecipientChain = recipientChain;

            this.TurnsInTransit = 0;
            this.TurnSent = turn_sent;
        }

        public void Tick() {
            this.TurnsInTransit++;
        }
    }

    public class DispatchState {
        [JsonIgnore]
        public static Random _rnd;

        [JsonIgnore]
        public ScenarioData Scenario { get; set; }

        [JsonProperty("current_dispatches")]
        public List<Dispatch> Dispatches { get; set; }

        [JsonProperty("dispatches_received")]
        public Dictionary<int, List<Dispatch>> DispatchesReceived { get; set; }

        [JsonProperty("dispatches_sent")]
        public Dictionary<int, Dictionary<string, int>> NumDispatchesSent { get; set; }

        [JsonProperty("current_turn")]
        public int CurrentTurn { get; set; }

        public string BattleFilename { get; set; }

        public string MapFilename { get; set; }

        public string OOBFilename { get; set; }

        public string PDTFilename { get; set; }

        [JsonProperty("scenario_settings")]
        public ScenarioSettings Settings { get; set; }

        public DispatchState() {
            if (_rnd == null) {
                _rnd = new Random();
            }
        }

        public DispatchState(ScenarioData sd) {
            Dispatches = new List<Dispatch>();
            DispatchesReceived = new Dictionary<int, List<Dispatch>>();
            NumDispatchesSent = new Dictionary<int, Dictionary<string, int>>();

            SetScenarioData(sd);

            NumDispatchesSent.Add(CurrentTurn, new Dictionary<string, int>());
            DispatchesReceived.Add(CurrentTurn, new List<Dispatch>());

            Settings = null;

            if (_rnd == null) {
                _rnd = new Random();
            }
        }

        public void SetScenarioData(ScenarioData sd) {
            Scenario = sd;
            CurrentTurn = Scenario.GetCurrentTurn();

            BattleFilename = Scenario.GetFilename();
            MapFilename = Scenario.GetMapFilename();
            PDTFilename = Scenario.GetPDTFilename();
            OOBFilename = Scenario.GetOOBFilename();
        }

        public void Serialize() {
            if(!Scenario.LoadedCorrectly()) {
                // Don't create a dispatch state on disk if the scenario data
                // wasn't loaded correctly
                return;
            }
            
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            string ext = "dispatch";
            if(Scenario.IsPBEM()) {
                ext += "_pem";
            }

            File.WriteAllText(
                Path.ChangeExtension(Scenario.GetFilenameFullPath(), ext), 
                json,
                Encoding.GetEncoding("ISO-8859-1")
            );
        }

        public static DispatchState Deserialize(string filename, TreeView treeRecip, TreeView treeSender) {
            ScenarioData sd = new ScenarioData();
            sd.LoadScenario(filename, treeRecip, treeSender);

            string ext = "dispatch";
            if(sd.IsPBEM()) {
                ext += "_pem";
            }

            string dispatch_file = Path.ChangeExtension(filename, ext);
            if(File.Exists(dispatch_file)) {
                string json = File.ReadAllText(
                    dispatch_file, 
                    Encoding.GetEncoding("ISO-8859-1")
                );

                DispatchState ds = JsonConvert.DeserializeObject<DispatchState>(json);

                if(
                    ds.CurrentTurn > sd.GetCurrentTurn() ||
                    ds.BattleFilename != sd.GetFilename() ||
                    ds.MapFilename != sd.GetMapFilename() ||
                    ds.PDTFilename != sd.GetPDTFilename() ||
                    ds.OOBFilename != sd.GetOOBFilename()
                ) {
                    // Invalid dispatch file, start over
                    return new DispatchState(sd);
                }

                // Otherwise, pick up where we left off
                ds.Scenario = sd;
                return ds;
            }

            // New game
            return new DispatchState(sd);
        }

        public int DispatchesSentBy(string sender) {
            Dictionary<string, int> dispatchesSentTurn = NumDispatchesSent[CurrentTurn];

            if(dispatchesSentTurn.ContainsKey(sender)) {
                return dispatchesSentTurn[sender];
            }

            dispatchesSentTurn.Add(sender, 0);
            return 0;
        }

        public void IncDispatchCount(string sender) {
            Dictionary<string, int> dispatchesSentTurn = NumDispatchesSent[CurrentTurn];

            if (dispatchesSentTurn.ContainsKey(sender)) {
                dispatchesSentTurn[sender]++;
            } else {
                dispatchesSentTurn.Add(sender, 1);
            }
        }

        public void DecDispatchCount(string sender) {
            Dictionary<string, int> dispatchesSentTurn = NumDispatchesSent[CurrentTurn];

            if (dispatchesSentTurn.ContainsKey(sender)) {
                dispatchesSentTurn[sender]--;
            } else {
                throw new Exception("Dispatch count for " + sender + " was already zero!");
            }
        }

        public Dispatch FindDispatch(string sender, string recipient) {
            foreach(Dispatch dispatch in Dispatches) {
                if (dispatch.TurnSent == CurrentTurn) {
                    if (dispatch.Sender == sender && dispatch.Recipient == recipient) {
                        return dispatch;
                    }
                }
            }

            return null;
        }

        public Dispatch UndoLastDispatch() {
            if (Dispatches.Count > 0) {
                Dispatch last = Dispatches[Dispatches.Count - 1];
                if (last.TurnSent == CurrentTurn) { 
                    Dispatches.RemoveAt(Dispatches.Count - 1);
                    DecDispatchCount(last.Sender);
                    
                    Serialize();
                    
                    return last;
                }
            }

            return null;
        }

        public bool SendDispatch(string sender, string recipient, string message, List<string> recipientChain) {
            Location sender_location = Scenario.GetUnitLocation(sender);
            Location recipient_location = Scenario.GetUnitLocation(recipient);

            if (sender_location.IsPresent() && 
                recipient_location.IsPresent() &&
                DispatchesSentBy(sender) < Settings.DispatchesPerLeader
            ) { // max dispatches per sender
                IncDispatchCount(sender);

                Dispatches.Add(
                    new Dispatch(
                        sender, 
                        recipient,
                        message, 
                        sender_location, 
                        CurrentTurn,
                        recipientChain
                    )
                );

                Serialize();

                return true;
            }
            return false;
        }

        public List<Dispatch> Update() {
            int turnInterval = Scenario.GetCurrentTurn() - CurrentTurn;

            if (turnInterval > 0) {
                List<Dispatch> newDispatches = new List<Dispatch>();

                for (int i = 0; i < turnInterval; i++) {
                    AdvanceTurn();

                    newDispatches.AddRange(DispatchesReceived[CurrentTurn]);
                }

                Serialize();
                return newDispatches;
            }

            return null;
        }

        private void AdvanceTurn() {
            CurrentTurn++;

            NumDispatchesSent.Add(CurrentTurn, new Dictionary<string, int>());

            List<Dispatch> dispatchesReceivedThisTurn = new List<Dispatch>();

            List<Dispatch> dispatchesToRemove = new List<Dispatch>();

            List<Dispatch> dispatchesToAdd = new List<Dispatch>();

            foreach (Dispatch dispatch in Dispatches) {
                dispatch.Tick();

                List<string> recipChain = dispatch.RecipientChain;
                Location recip_location = Scenario.GetUnitLocation(dispatch.Recipient);
                if (!recip_location.IsPresent()) {
                    // recipient has left the scenario or has been killed
                    // dispatch will not be delivered
                    Location sender_location = Scenario.GetUnitLocation(dispatch.Sender);
                    if (sender_location.IsPresent()) {
                        dispatchesToAdd.Add(
                            new Dispatch(
                                "aide de camp",
                                dispatch.Sender,
                                "The following order could not be delivered to " +
                                    dispatch.Recipient +
                                    ", whose HQ could not be found:\n\n\"" +
                                    dispatch.Message + "\"",
                                dispatch.CurrentLocation,
                                CurrentTurn,
                                new List<string>()
                            )
                        );
                    }
                    dispatchesToRemove.Add(dispatch);
                } else {
                    // we've reached our recipient if we're at the recipient's location, and have
                    // exhausted our recipient chain, or else are not using strict chain of command
                    bool recipientReached = dispatch.CurrentLocation.Equals(recip_location) &&
                        (recipChain.Count == 0 || !Settings.UseChainOfCommand);

                    if (!recipientReached) {
                        // If we haven't already reached our recipient
                        int chance = _rnd.Next(1, 100);
                        if (chance <= Settings.ChanceDispatchLost) {
                            // Dispatch was lost!
                            dispatchesToRemove.Add(dispatch);
                        } else {
                            Location target_location;
                            if (recipChain.Count > 0 && Settings.UseChainOfCommand) {
                                target_location = Scenario.GetUnitLocation(recipChain[0]);
                                while(!target_location.IsPresent()) {
                                    // Route around any intervening HQs that may have disappeared
                                    recipChain.RemoveAt(0);
                                    if (recipChain.Count > 0) {
                                        target_location = Scenario.GetUnitLocation(recipChain[0]);
                                    } else {
                                        target_location = recip_location;
                                        break;
                                    }
                                }
                            } else {
                                target_location = recip_location;
                            }

                            for (int i = 0; i < Settings.DispatchSpeed; i++) {
                                dispatch.CurrentLocation = dispatch.CurrentLocation.MoveTowards(target_location, 1);

                                if (dispatch.CurrentLocation.Equals(target_location)) {
                                    if (recipChain.Count > 0 && Settings.UseChainOfCommand) {
                                        recipChain.RemoveAt(0);
                                        if (recipChain.Count > 0) {
                                            Location recipTest = Scenario.GetUnitLocation(recipChain[0]);
                                            while (!recipTest.IsPresent()) {
                                                // Route around any intervening HQs that may have disappeared
                                                recipChain.RemoveAt(0);
                                                if (recipChain.Count > 0) {
                                                    recipTest = Scenario.GetUnitLocation(recipChain[0]);
                                                } else {
                                                    recipTest = recip_location;
                                                    break;
                                                }
                                            }
                                            target_location = recipTest;
                                        } else {
                                            target_location = recip_location;
                                        }
                                        if (dispatch.CurrentLocation.Equals(recip_location)) {
                                            recipientReached = true;
                                            break;
                                        }
                                    } else {
                                        // we've arrived
                                        recipientReached = true;
                                        break;
                                    }
                                } else {
                                    if (Settings.InterdictionChance > 0) {
                                        if (Scenario.InEnemyZOC(dispatch.CurrentLocation)) {
                                            chance = _rnd.Next(1, 100);
                                            if (chance <= Settings.InterdictionChance) {
                                                // Dispatch was interdicted!
                                                dispatchesToRemove.Add(dispatch);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (recipientReached) {
                        // Dispatch has arrived
                        if (dispatch.TurnsInTransit >= Settings.MinimumDispatchDelay) {
                            // Deliver it
                            dispatchesReceivedThisTurn.Add(dispatch);
                            dispatchesToRemove.Add(dispatch);
                        }
                    }
                }
            }

            foreach(Dispatch dispatch in dispatchesToRemove) {
                Dispatches.Remove(dispatch);
            }
            dispatchesToRemove.Clear();
            foreach(Dispatch dispatch in dispatchesToAdd) {
                Dispatches.Add(dispatch);
            }

            DispatchesReceived.Add(CurrentTurn, dispatchesReceivedThisTurn);
        }

        public List<Dispatch> GetDispatchesReceived(int turn = -1) { 
            if(turn == -1) {
                turn = CurrentTurn;
            }
            
            if(DispatchesReceived.ContainsKey(turn)) {
                return DispatchesReceived[turn];
            }

            return new List<Dispatch>();
        }
    }
}
