# CompatabilityRulePacker

This project is an algorithm to estimate an optimal sorting of items into groups (using as few groups as possible) where some items are incompatible and cannot be grouped with each other. After starting this algorithm I realised that this problem is the same as the [Graph Coloring](https://en.wikipedia.org/wiki/Graph_coloring) problem. 

This is because the item compatability rules can be represented as a graph where each item is a node and each connection is a pair of items that are incompatible. We need to assign each item a category so that incompatible items (adjacent nodes) are not assigned the same category. This is the Graph Coloring problem.

Determining the minimum number of colours/groups required (the chromatic number), and by extension finding a valid grouping with this number, is an NP-Hard problem, meaning there is no efficient algorithm to do this (in polynomial time). This project is an algorithm which provides an estimate, and is a version of the [Greedy Coloring](https://en.wikipedia.org/wiki/Greedy_coloring) algorithm, with the ordering of nodes being a BFS from an arbitrary starting node.