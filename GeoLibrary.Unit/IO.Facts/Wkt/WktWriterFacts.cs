﻿using System;
using FluentAssertions;
using GeoLibrary.IO.Wkt;
using GeoLibrary.Model;
using Xunit;

namespace GeoLibrary.Unit.IO.Wkt.Facts
{
    public class WktWriterFacts
    {
        [Fact]
        public void If_geometry_is_invalid_then_should_throw_exception()
        {
            var point = new Point();

            Assert.Throws<ArgumentException>(() => WktWriter.Write(point)).Message.Should()
                .BeEquivalentTo("Invalid geometry");
        }

        [Fact]
        public void If_point_is_valid_then_should_return_wkt()
        {
            var point = new Point(10, 20);
            const string expectWkt = "POINT (10 20)";

            var resultWkt = WktWriter.Write(point);
            resultWkt.Should().BeEquivalentTo(expectWkt);
            point.ToWkt().Should().BeEquivalentTo(expectWkt);
        }

        [Fact]
        public void If_multipoint_is_valid_then_should_return_wkt()
        {
            var multiPoint = new MultiPoint(new []{ new Point(10, 20), new Point(20, 30), new Point(30, 60) });
            const string expectWkt = "MULTIPOINT (10 20, 20 30, 30 60)";

            WktWriter.Write(multiPoint).Should().BeEquivalentTo(expectWkt);
        }

        [Fact]
        public void If_linestring_is_valid_then_should_return_wkt()
        {
            var lineString = new LineString(new [] { new Point(10, 20), new Point(10, 30), new Point(0, 0) });
            const string expectWkt = "LINESTRING (10 20, 10 30, 0 0)";

            WktWriter.Write(lineString).Should().BeEquivalentTo(expectWkt);
        }

        [Fact]
        public void If_polygon_with_one_ring_is_valid_then_should_return_wkt()
        {
            var polygon = new Polygon(new[] { new LineString(new[] { new Point(-120, 30), new Point(0, 0), new Point(120, 30), new Point(-120, 30) }) });
            const string expectWkt = "POLYGON ((-120 30, 0 0, 120 30, -120 30))";

            WktWriter.Write(polygon).Should().BeEquivalentTo(expectWkt);
        }

        [Fact]
        public void If_multipolygon_is_valid_then_should_return_wkt()
        {
            var multiPolygon = new MultiPolygon(new []
            {
                new Polygon(new[] { new LineString(new[] { new Point(30, 20), new Point(45, 40), new Point(10, 40), new Point(30, 20) }) }),
                new Polygon(new[] { new LineString(new[] { new Point(15, 5), new Point(40, 10), new Point(10, 20), new Point(5, 10), new Point(15, 5) }) })
            });
                
            const string expectWkt = "MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))";

            WktWriter.Write(multiPolygon).Should().BeEquivalentTo(expectWkt);
        }
    }
}
