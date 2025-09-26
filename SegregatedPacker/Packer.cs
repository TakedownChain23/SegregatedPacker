namespace SegregatedPacker
{
    public static class Packer
    {
        public static (List<List<T>> containers, List<T> unassignedItems) PackSegregatedItems<T>(IEnumerable<T> items, Func<T, T, bool> itemsAreCompatible, int maxContainers = 100)
            where T : notnull
        {
            Dictionary<T, List<T>> incompatibleItemsForItem = [];
            Dictionary<T, int?> assignedContainerForItem = [];
            var maxContainer = 0;

            if (!items.Any()) return ([], []);

            List<T> itemsRequiringSegregation = [];
            List<T> itemsNotRequiringSegregation = [];
            foreach (var item in items)
            {
                var incompatibleItems = items.Where(i => !itemsAreCompatible(i, item));
                if (incompatibleItems.Any())
                {
                    itemsRequiringSegregation.Add(item);
                }
                else
                {
                    itemsNotRequiringSegregation.Add(item);
                }

                assignedContainerForItem[item] = null;
                incompatibleItemsForItem[item] = [.. incompatibleItems];
            }

            foreach (var item in itemsRequiringSegregation)
            {
                if (assignedContainerForItem[item] == null)
                {
                    var itemQueue = new Queue<T>();

                    assignedContainerForItem[item] = 0;
                    itemQueue.Enqueue(item);
                    while (itemQueue.Count > 0)
                    {
                        var nextItem = itemQueue.Dequeue();

                        var incompatableItems = incompatibleItemsForItem[nextItem];
                        foreach (var incompatableItem in incompatableItems.Where(i => assignedContainerForItem[i] == null))
                        {
                            var lowestValidContainer = GetLowestValidContainer(incompatableItem, maxContainers);
                            
                            assignedContainerForItem[incompatableItem] = lowestValidContainer;
                            if (maxContainer < lowestValidContainer) maxContainer = lowestValidContainer;

                            itemQueue.Enqueue(incompatableItem);
                        }
                    }
                }
            }

            var result = new List<List<T>>(maxContainer);
            for (int i = 0; i <= maxContainer; i++)
            {
                result.Add([]);
            }

            foreach (var item in itemsRequiringSegregation)
            {
                var assignedContainer = assignedContainerForItem[item];
                if (assignedContainer is int container)
                {
                    result[container].Add(item);
                }
            }

            return (result, itemsNotRequiringSegregation);

            int GetLowestValidContainer(T item, int maxContainers)
            {
                var incompatibleContainers = incompatibleItemsForItem[item].Select(i => assignedContainerForItem[i]).Where(c => c != null).Distinct();
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
