namespace CompatabilityRulePacker.Model
{
    internal class Node<T>
    {
        public required T Value { get; set; }
        public int? AssignedBin { get; set; }
        public List<Node<T>> IncompatableNodes { get; set; } = [];
    }
}
