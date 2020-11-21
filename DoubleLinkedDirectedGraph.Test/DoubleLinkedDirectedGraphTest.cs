using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xunit;

namespace DoubleLinkedDirectedGraph.Test
{
    public class DoubleLinkedDirectedGraphTest
    {
        [Fact]
        public void GraphEndpointMatching_InsertNotFromStart()
        {
            DoubleLinkedDirectedGraph<string, string> graph = new DoubleLinkedDirectedGraph<string, string>();
            graph.InsertFromStart("1").Insert("2").Insert("3");
            graph.Insert("1","4").Insert("3");
            graph.FinishGraph();
            (graph.Start).Count().ShouldBe(1);
            (graph.End).Count().ShouldBe(1);
        }

        [Fact]
        public void GraphEndpointMatching_InsertFromStart()
        {
            DoubleLinkedDirectedGraph<string, string> graph = new DoubleLinkedDirectedGraph<string, string>();
            graph.InsertFromStart("i").Insert("n").Insert("t");
            graph.InsertFromStart("i").Insert("c").Insert("e");
            graph.FinishGraph();
            (graph.Start).Count().ShouldBe(1);
            (graph.End).Count().ShouldBe(2);
        }

        [Fact]
        public void GraphEndpointMatching_InsertWithExplicitEndPoint()
        {
            DoubleLinkedDirectedGraph<string, string> graph = new DoubleLinkedDirectedGraph<string, string>();
            graph.InsertFromStart("omo").Insert("moma").InsertEnd();
            graph.Insert("moma", "merm").InsertEnd();
            graph.Insert("omo", "mama").Insert("merm").InsertEnd();
            graph.Insert("mama","marm").InsertEnd();
            graph.InsertFromStart("ohm").InsertEnd();
            graph.FinishGraph();
            (graph.Start).Count().ShouldBe(2);
            (graph.End).Count().ShouldBe(4);
        }

        //[Fact]
        //public void GraphEndpointMatching_InsertFromStart_OptionsLocallyUnique()
        //{
        //    DoubleLinkedDirectedGraph<string, string> graph = new DoubleLinkedDirectedGraph<string, string>(new DoubleLinkedDirectedGraph<string, string>.Options() { TreatNodeKeysAsOnlyLocallyUnique = true });
        //    graph.InsertFromStart("i").Insert("n").Insert("t");
        //    graph.InsertFromStart("i").Insert("n").Insert("i");
        //    graph.FinishGraph();
        //    (graph.Start).Count().ShouldBe(1);
        //    (graph.End).Count().ShouldBe(2);
        //}

        //[Fact]
        //public void GraphEndpointMatching_InsertWithExplicitEndPoint_OptionsLocallyUnique()
        //{
        //    DoubleLinkedDirectedGraph<string, string> graph = new DoubleLinkedDirectedGraph<string, string>();
        //    graph.InsertFromStart("projection1").Insert("projection2").InsertEnd();
        //    graph.InsertFromStart("projection1").Insert("projection3").InsertEnd();
        //    graph.InsertFromStart("projection2").Insert("projection3").InsertEnd();
        //    graph.InsertFromStart("projection1").InsertEnd();
        //    graph.FinishGraph();
        //    (graph.Start).Count().ShouldBe(2);
        //    (graph.Start).First().NextEdges.Values.Count().ShouldBe(3);
        //    (graph.Start).Skip(1).First().NextEdges.Values.Count().ShouldBe(1);
        //    (graph.End).Count().ShouldBe(3);
        //}
    }
}
