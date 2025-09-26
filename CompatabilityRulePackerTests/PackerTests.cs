using CompatabilityRulePacker;

namespace CompatabilityRulePackerTests
{
    public class PackerTests
    {
        static readonly int[] items = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        [TestCaseSource(nameof(PackItemsWithCompatabilityRulesTestCases))]
        public void PackItemsWithCompatabilityRulesTest(Func<int, int, bool> itemsAreCompatable, int[] expectedUnassignedItems, int expectedContainerCount)
        {
            (var containers, var unassignedItems) = Packer.PackItemsWithCompatabilityRules(items, itemsAreCompatable);

            Assert.That(unassignedItems, Is.EquivalentTo(expectedUnassignedItems));
            Assert.That(containers, Has.Count.EqualTo(expectedContainerCount));

            Assert.That(containers.SelectMany(c => c).Concat(unassignedItems), Is.EquivalentTo(items));
            foreach (var container in containers)
            {
                for (var i = 0; i < container.Count; i++)
                {
                    for (var j = i + 1; j < container.Count; j++)
                    {
                        Assert.That(itemsAreCompatable(container[i], container[j]));
                    }
                }
            }
        }

        public static IEnumerable<TestCaseData> PackItemsWithCompatabilityRulesTestCases
        {
            get
            {
                yield return new TestCaseData(
                    (int i1, int i2) => true,
                    items,
                    1
                ).SetName("All items compatible - {2} Container(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => false,
                    Array.Empty<int>(),
                    10
                ).SetName("All items incompatible - {2} Container(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => Math.Abs(i2 - i1) != 1,
                    Array.Empty<int>(),
                    2
                ).SetName("Chain incompatibility - {2} Container(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 > 5 || (i1 + 4) % 5 != i2 && (i1 + 6) % 5 != i2,
                    new int[] { 5, 6, 7, 8, 9 },
                    3
                ).SetName("Loop of five incompatibility - {2} Container(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 * i2 != 0,
                    Array.Empty<int>(),
                    2
                ).SetName("All incompatible with one item - {2} Container(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 / 2 == i2 / 2 && Math.Abs(i1 - i2) == 1,
                    Array.Empty<int>(),
                    5
                ).SetName("Adjacent pairs only - {2} Container(s)");
            }
        }
    }
}