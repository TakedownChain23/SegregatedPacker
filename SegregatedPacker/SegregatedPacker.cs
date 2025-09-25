namespace SegregatedPacker
{
    public static class SegregatedPacker<T> where T : notnull
    {
        public static List<List<T>> PackSegregatedItems(IEnumerable<T> items, Func<T, T, bool> itemsAreCompatible, int maxContainers = 100)
        {
            Dictionary<T, List<T>> incompatibleItems = [];
            Dictionary<T, int?> assignedContainers = [];
            var maxContainer = 0;

            if (!items.Any()) return [];
            
            foreach (var item in items)
            {
                incompatibleItems[item] = [.. items.Where(i => !itemsAreCompatible(i, item))];
                assignedContainers[item] = null;
            }

            foreach (var item in items)
            {
                if (assignedContainers[item] == null)
                {
                    var itemQueue = new Queue<T>();

                    assignedContainers[item] = 0;
                    itemQueue.Enqueue(item);
                    while (itemQueue.Count > 0)
                    {
                        var nextItem = itemQueue.Dequeue();

                        var incompatableItems = incompatibleItems[nextItem];
                        foreach (var incompatableItem in incompatableItems.Where(i => i == null))
                        {
                            var lowestValidContainer = GetLowestValidContainer(incompatableItem, maxContainers);
                            
                            assignedContainers[incompatableItem] = lowestValidContainer;
                            if (maxContainer < lowestValidContainer) maxContainer = lowestValidContainer;

                            itemQueue.Enqueue(incompatableItem);
                        }
                    }
                }
            }

            var result = Enumerable.Repeat(new List<T>(), maxContainer).ToList();
            foreach (var item in items)
            {
                var assignedContainer = assignedContainers[item];
                if (assignedContainer is int container)
                {
                    result[container].Add(item);
                }
            }

            return result;

            int GetLowestValidContainer(T item, int maxContainers)
            {
                var incompatibleContainers = incompatibleItems[item].Select(i => assignedContainers[i]).Where(c => c != null).Distinct();
                var container = 0;
                while (incompatibleContainers.Contains(container))
                {
                    container++;
                    if (container >= maxContainers) throw new Exception("Items require too many containers. Increase maxContainers if required.");
                }

                return container;
            }
        }
    }
}
