using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

public class Day20 : BaseDay
{
    private readonly List<string> _input = new();

    public Day20()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        int lowPulses = 1000;
        int highPulses = 0;
        
        Dictionary<string, Node> nodes = new();
        OrderedDictionary connections = new();
        Queue<(Node sender, Node receiver, bool lowOrHigh)> pulseQueue = new();
        
        foreach (var line in _input)
        {
            // Handle new nodes
            Node newNode = null;
            string name = "broadcaster";
            if (line[0] == '%') // flip-flop
            {
                name = line.Split(" -> ")[0].Replace("%", "");
                newNode = new FlipFlop();
            }
            else if (line[0] == '&') // conjunction
            {
                name = line.Split(" -> ")[0].Replace("&", "");
                newNode = new Conjunction();
            }
            else if (line.StartsWith("broadcaster")) // broadcaster
            {
                name = "broadcaster";
                newNode = new Broadcaster();
            }
            else
            {
                name = line.Split(" -> ")[0].Trim();
                newNode = new Node();
            }
            
            newNode.Name = name;
            nodes[name] = newNode;
            connections[name] = new List<string>();
            
            // Handle connections
            var connectedNodeNames = line.Split(" -> ")[1].Split(", ");
            foreach (var connectedNodeName in connectedNodeNames)
            {
                (connections[name] as List<string>).Add(connectedNodeName);
            }
            
            newNode.SendPulse = (sender, lowOrHigh) =>
            {
                //Console.WriteLine($"High: {highPulses}, Low: {lowPulses}");
                
                var connectedNodeNames = (List<string>)connections[sender.Name];

                foreach (var connectedNodeName in connectedNodeNames)
                {
                    if (nodes.TryGetValue(connectedNodeName, out var node))
                    {
                        pulseQueue.Enqueue((sender, node, lowOrHigh));
                    }
                }
            };
        }

        foreach (var key in connections.Keys)
        {
            var connectionList = (List<string>)connections[key];

            foreach (var connectionName in connectionList)
            {
                if (!nodes.ContainsKey(connectionName))
                    nodes[connectionName] = new Node() { Name = connectionName };
                
                nodes[connectionName].ConnectInput(nodes[(string)key]);
            }
        }
        
        var broadcastNode = nodes["broadcaster"];
        while(true)
        {
            broadcastNode.ReceivePulse(null, false);

            while (pulseQueue.Count > 0)
            {
                var (sender, receiver, lowOrHigh) = pulseQueue.Dequeue();
                //Console.WriteLine($"{sender.Name} -> {receiver.Name} ({lowOrHigh})");
                receiver.ReceivePulse(sender, lowOrHigh);
                if (lowOrHigh)
                    highPulses++;
                else
                    lowPulses++;
            }
        }
        
        return new((lowPulses * highPulses).ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        return new("");
    }
    private int solve()
    {
        return 0;
    }

    class Node
    {
        public string Name;
        public List<Node> Inputs = new();
        public List<Node> Outputs = new();
        public Action<Node, bool> SendPulse = (sender, lowOrHigh) => { };

        public virtual void ConnectInput(Node newInput)
        {
            Inputs.Add(newInput);
        }

        public virtual void ConnectOutput(Node newOutput)
        {
            Outputs.Add(newOutput);
        }

        public virtual void ReceivePulse(Node sender, bool lowOrHigh)
        {
            if (!lowOrHigh && this.Name == "rx")
                Debugger.Break();
        }
    }

    class FlipFlop : Node
    {
        public bool State = false;

        public override void ReceivePulse(Node sender, bool lowOrHigh)
        {
            if (lowOrHigh)
                return;
            
            State = !State;
            SendPulse(this, State);
        }
    }
    
    class Broadcaster : Node
    {
        public override void ReceivePulse(Node sender, bool lowOrHigh)
        {
            SendPulse(this, lowOrHigh);
        }
    }

    class Conjunction : Node
    {
        Dictionary<Node, bool> lastReceived = new();

        public override void ReceivePulse(Node sender, bool lowOrHigh)
        {
            lastReceived[sender] = lowOrHigh;

            if (lastReceived.Values.All(x => x))
                SendPulse(this, false);
            else
                SendPulse(this, true);
        }

        public override void ConnectInput(Node newInput)
        {
            lastReceived.Add(newInput, false);
            base.ConnectInput(newInput);
        }
    }
}