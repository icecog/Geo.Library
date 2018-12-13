using FluentAssertions;
using GeoLibrary.Model;
using System;
using Xunit;

namespace GeoLibrary.Unit.Model.Facts
{
    public class PointFacts
    {
        [Fact]
        public void If_point_is_not_initial_then_it_should_be_invalid()
        {
            var point = new Point();

            point.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(-180.1, 90)]
        [InlineData(180.0001, -70)]
        [InlineData(-170, 100)]
        [InlineData(120, -91)]
        public void If_point_with_out_of_range_latlng_then_it_should_be_invalid(double longitude, double latitude)
        {
            var point = new Point(longitude, latitude);

            point.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(-170.1, 90)]
        [InlineData(180.000000001, -70)]
        [InlineData(120, -90.000000001)]
        public void If_point_with_valid_latlng_then_it_should_be_valid(double longitude, double latitude)
        {
            var point = new Point(longitude, latitude);

            point.IsValid.Should().BeTrue();
        }

        [Fact]
        public void A_point_should_not_equal_to_a_none_point()
        {
            var point = new Point();
            var other = 100;

            point.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void A_valid_point_should_not_equal_an_invalid_point()
        {
            var point = new Point(100, 50);
            var other = new Point();

            point.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void Two_valid_points_with_different_latlng_should_not_equal()
        {
            var point = new Point(100, 50);
            var other = new Point(100, 60);

            point.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void Two_valid_points_with_same_latlng_should_equal()
        {
            var point = new Point(100, 50);
            var other = new Point(100, 50);

            point.Equals(other).Should().BeTrue();
            (point == other).Should().BeTrue();
        }

        [Fact]
        public void One_valid_point_should_equal_to_itself()
        {
            var point = new Point(100, 50);

            point.Equals(point).Should().BeTrue();
        }

        [Fact]
        public void Two_valid_points_with_very_close_latlng_should_equal()
        {
            var point = new Point(100.000000003, 50);
            var other = new Point(100, 50.000000003);

            point.Equals(other).Should().BeTrue();
            (point == other).Should().BeTrue();
        }

        [Fact]
        public void Two_null_points_should_equal()
        {
            Point point = null, other = null;

            (point == other).Should().BeTrue();
        }

        [Fact]
        public void One_null_point_should_not_equal_a_none_null_point()
        {
            Point point = new Point(), other = null;

            (point == other).Should().BeFalse();
        }

        [Fact]
        public void One_none_null_point_should_not_equal_a_null_point()
        {
            Point point = null, other = new Point();

            (point == other).Should().BeFalse();
        }

        [Fact]
        public void Clone_an_invalid_point_should_be_an_invalid_point()
        {
            var point = new Point();

            var copied = point.Clone();

            copied.IsValid.Should().BeFalse();
            (copied is Point).Should().BeTrue();
        }

        [Fact]
        public void Clone_a_valid_point_should_equal()
        {
            var point = new Point(100, 50);

            point.Clone().Equals(point).Should().BeTrue();
        }

        [Fact]
        public void Same_point_union_should_get_clone_of_itself()
        {
            var point = new Point(100, 50);

            var unionPoint = point.Union(new Point(100, 50));

            unionPoint.IsValid.Should().BeTrue();
            unionPoint.Equals(point).Should().BeTrue();
            ReferenceEquals(point, unionPoint).Should().BeFalse();
        }

        [Fact]
        public void Different_points_union_should_get_multipoint()
        {
            var point1 = new Point(100, 50);
            var point2 = new Point(120, 60);

            var unionGeo = point1.Union(point2);
            var unionGeo2 = point2.Union(point1);

            (unionGeo is MultiPoint).Should().BeTrue();
            (unionGeo2 is MultiPoint).Should().BeTrue();

            var multiPoint = unionGeo as MultiPoint;
            multiPoint.Count.Should().Be(2);
        }

        [Fact]
        public void Valid_point_union_invalid_one_should_get_clone_of_valid_one()
        {
            var point1 = new Point(100, 50);
            var point2 = new Point();

            var unionGeo = point1.Union(point2);
            var unionGeo2 = point2.Union(point1);

            (unionGeo is Point).Should().BeTrue();
            unionGeo2.Equals(unionGeo).Should().BeTrue();

            unionGeo.Equals(point1).Should().BeTrue();
        }

        [Fact]
        public void Point_union_multipoint_should_be_multipoint()
        {
            var point = new Point(100, 50);
            var multiPoint = new MultiPoint(new[] { new Point(100, 50), new Point(120, -70) });

            var unionGeo = point.Union(multiPoint);
            var unionGeo2 = multiPoint.Union(point);

            (unionGeo is MultiPoint).Should().BeTrue();
            unionGeo2.Equals(unionGeo).Should().BeTrue();

            unionGeo.Equals(multiPoint).Should().BeTrue();
        }

        [Fact]
        public void Point_union_linestring_should_not_support()
        {
            var point = new Point();
            var linestring = new LineString();

            Assert.Throws<Exception>(() => point.Union(linestring)).Message.Should().Be("Not supported type!");
        }

        [Fact]
        public void Invalid_points_should_not_be_intersects()
        {
            var point = new Point();
            var point2 = new Point();

            point.IsIntersects(point2).Should().BeFalse();

            var result = point.Intersection(point2);
            result.Should().Be(null);
        }

        [Fact]
        public void Invalid_point_should_not_intersect_valid_point()
        {
            var point1 = new Point(100, 50);
            var point2 = new Point();

            point1.IsIntersects(point2).Should().BeFalse();

            var result = point1.Intersection(point2);
            result.Should().Be(null);
        }

        [Fact]
        public void Different_points_should_not_intersect()
        {
            var point1 = new Point(100, 50);
            var point2 = new Point(120, 60);

            point1.IsIntersects(point2).Should().BeFalse();

            var result = point1.Intersection(point2);
            result.Should().Be(null);
        }

        [Fact]
        public void Same_points_should_intersect()
        {
            var point1 = new Point(100, 50);
            var point2 = new Point(100, 50);

            point1.IsIntersects(point2).Should().BeTrue();

            var result = point1.Intersection(point2);
            result.Should().NotBe(null);
            result.Equals(point1).Should().BeTrue();
        }

        [Fact]
        public void Valid_point_should_not_be_intersect_empty_multipoint()
        {
            var point1 = new Point(100, 50);
            var multiPoint = new MultiPoint();

            point1.IsIntersects(multiPoint).Should().BeFalse();

            var result = point1.Intersection(multiPoint);
            result.Should().Be(null);
        }

        [Fact]
        public void Invalid_point_should_not_be_intersect_valid_multipoint()
        {
            var point1 = new Point();
            var multiPoint = new MultiPoint(new[] { new Point(100, 50), new Point(120, -70) });

            point1.IsIntersects(multiPoint).Should().BeFalse();

            var result = point1.Intersection(multiPoint);
            result.Should().Be(null);
        }
    }
}
