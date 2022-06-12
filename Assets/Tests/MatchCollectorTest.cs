using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MatchCollectorTest
    {
        [Test]
        public void TestNoMatch([Values(0, 1, 2)]int tapRow, [Values(0, 1, 2)]int tapCol) {
            // Setup
            int[,] grid = {
                {0,1,2},
                {3,0,5},
                {0,7,8}
            };
            MatchCollector matchCollector = new MatchCollector(grid);
            matchCollector.Init(rows: 3, columns: 3);
            
            // Action
            List<GridPosition> matches = matchCollector.DetectMatches(tapRow, tapCol);
            
            // Assert - returning only the same tapped cell
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0].row == tapRow);
            Assert.IsTrue(matches[0].col == tapCol);
        }
        
        [TestCase(0,0, 5)] [TestCase(0,1, 5)] [TestCase(0,2, 1)]
        [TestCase(1,0, 1)] [TestCase(1,1, 5)] [TestCase(1,2, 2)]
        [TestCase(2,0, 5)] [TestCase(2,1, 5)] [TestCase(2,2, 2)]
        public void TestMatchLength(int tapRow, int tapCol, int expectedMatchLength) {
            // Setup
            int[,] grid = {
                {0,0,2},
                {3,0,5},
                {0,0,5}
            };
            MatchCollector matchCollector = new MatchCollector(grid);
            matchCollector.Init(rows: 3, columns: 3);
            
            // Action
            List<GridPosition> matches = matchCollector.DetectMatches(tapRow, tapCol);
            
            // Assert
            Assert.IsTrue(matches.Count == expectedMatchLength);
        }
    }
}
