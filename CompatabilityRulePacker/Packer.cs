using CompatabilityRulePacker.Model;

namespace CompatabilityRulePacker
{
    public static class Packer
    {
        public static (T[][] bins, T[] unassignedItems) PackItemsWithCompatabilityRules<T>(IEnumerable<T> items, Func<T, T, bool> itemsAreCompatible, int maxBins = 100)
        {
            Node<T>[] incompatabilityGraphNodes = [..items.Select(i => new Node<T> { Value = i })];

            AddIncompatibilityConnectionsToGraph(incompatabilityGraphNodes, itemsAreCompatible);

            foreach (var node in incompatabilityGraphNodes.Where(n => n.IncompatableNodes.Count != 0))
            {
                if (node.AssignedBin == null)
                {
                    AssignBinsToConnectedNodesInIncompatibilityGraph(node, maxBins);
                }
            }

            var bins = incompatabilityGraphNodes.Where(n => n.AssignedBin != null)
                .GroupBy(n => n.AssignedBin)
                .Select(g => g.Select(n => n.Value).ToArray())
                .ToArray();

            var unassignedItems = incompatabilityGraphNodes.Where(n => n.AssignedBin == null)
                .Select(n => n.Value)
                .ToArray();

            return (bins, unassignedItems);
        }

        static void AddIncompatibilityConnectionsToGraph<T>(Node<T>[] graphNodes, Func<T, T, bool> itemsAreCompatible)
        {
            for (var i = 0; i < graphNodes.Length; i++)
            {
                for (var j = i + 1; j < graphNodes.Length; j++)
                {
                    var node1 = graphNodes[i];
                    var node2 = graphNodes[j];

                    if (!itemsAreCompatible(node1.Value, node2.Value))
                    {
                        node1.IncompatableNodes.Add(node2);
                        node2.IncompatableNodes.Add(node1);
                    }
                }
            }
        }

        static void AssignBinsToConnectedNodesInIncompatibilityGraph<T>(Node<T> startingNode, int maxBins)
        {
            startingNode.AssignedBin = 0;
            Queue<Node<T>> nodeQueue = [];
            nodeQueue.Enqueue(startingNode);

            while (nodeQueue.Count > 0)
            {
                var node = nodeQueue.Dequeue();
                foreach (var incompatableNode in node.IncompatableNodes.Where(n => n.AssignedBin == null))
                {
                    incompatableNode.AssignedBin = GetLowestValidBin(incompatableNode, maxBins);
                    nodeQueue.Enqueue(incompatableNode);
                }
            }
        }

        static int GetLowestValidBin<T>(Node<T> node, int maxBins)
        {
            var incompatibleBins = node.IncompatableNodes.Select(n => n.AssignedBin).Distinct();
            var lowestValidBin = 0;
            while (incompatibleBins.Contains(lowestValidBin))
            {
                lowestValidBin++;
                if (lowestValidBin >= maxBins)
                {
                    throw new Exception("Items require too many bins. Increase maxBins if required.");
                }
            }

            return lowestValidBin;
        }
    }
}
