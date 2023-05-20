using System;
using FluentAssertions;
using System.Linq;
using Xunit;
using ClassLibrary;

namespace Test_ClassLibrary
{
    public class Test_CL
    {
        public double leftEnd = 1;
        public double rightEnd = 10;
        public int nRawNodes = 10;
        public bool uni = true;
        public int nGrid = 10;
        public double leftdif = 0;
        public double rightdif = 1;
        public FRaw fRaw = FRawFunctions.linear;
        public RawData rawData;
        public SplineData splineData;


        [Fact]
        public void BasicTest_linear()
        {
            leftdif = 0;
            rightdif = 0;

            rawData = new RawData(leftEnd, rightEnd, nRawNodes, uni, fRaw);
            splineData = new SplineData(rawData, leftdif, rightdif, nGrid);
            splineData.spline();
            (rawData.x).SkipLast(1).Zip((rawData.x).Skip(1)).Should().OnlyContain(t => Math.Abs(t.Second - t.First - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (rawData.x).Zip((rawData.y)).Should().OnlyContain(t => Math.Abs(t.First - t.Second) < 1e-10);
            (splineData.data).SkipLast(1).Zip((splineData.data).Skip(1)).Should().OnlyContain(t => 
                        Math.Abs(t.Second.x - t.First.x - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (splineData.data).Zip((splineData.data)).Should().OnlyContain(t => Math.Abs(t.First.x - t.Second.y) < 1e-10);
        }
        [Fact]
        public void BasicTest_linear_nonuni()
        {
            leftdif = 0;
            rightdif = 0;
            uni = false;
            rawData = new RawData(leftEnd, rightEnd, nRawNodes, uni, fRaw);
            splineData = new SplineData(rawData, leftdif, rightdif, nGrid);
            splineData.spline();
            (rawData.x).SkipLast(1).Zip((rawData.x).Skip(1)).Should().OnlyContain(t => t.First < t.Second);
            (rawData.x).Zip((rawData.y)).Should().OnlyContain(t => Math.Abs(t.First - t.Second) < 1e-10);
            (splineData.data).SkipLast(1).Zip((splineData.data).Skip(1)).Should().OnlyContain(t =>
                        Math.Abs(t.Second.x - t.First.x - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (splineData.data).Zip((splineData.data)).Should().OnlyContain(t => Math.Abs(t.First.x - t.Second.y) < 1e-10);
        }
        [Fact]
        public void BasicTest_cube()
        {
            leftdif = 6 * leftEnd;
            rightdif = 6 * rightEnd;
            fRaw = FRawFunctions.cube;
            rawData = new RawData(leftEnd, rightEnd, nRawNodes, uni, fRaw);
            splineData = new SplineData(rawData, leftdif, rightdif, nGrid);
            splineData.spline();
            (rawData.x).SkipLast(1).Zip((rawData.x).Skip(1)).Should().OnlyContain(t => Math.Abs(t.Second - t.First - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (rawData.x).Zip((rawData.y)).Should().OnlyContain(t => Math.Abs(t.First * t.First * t.First - t.Second) < 1e-10);
            (splineData.data).SkipLast(1).Zip((splineData.data).Skip(1)).Should().OnlyContain(t =>
                        Math.Abs(t.Second.x - t.First.x - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (splineData.data).Zip((splineData.data)).Should().OnlyContain(t => Math.Abs(t.First.x * t.First.x * t.First.x - t.Second.y) < 1e-10);
        }

        [Fact]
        public void BasicTest_rand()
        {
            fRaw = FRawFunctions.rand;
            rawData = new RawData(leftEnd, rightEnd, nRawNodes, uni, fRaw);
            splineData = new SplineData(rawData, leftdif, rightdif, nGrid);
            splineData.spline();
            (rawData.x).SkipLast(1).Zip((rawData.x).Skip(1)).Should().OnlyContain(t => Math.Abs(t.Second - t.First - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (rawData.x).Should().OnlyContain(t => ((-10 < t) && (t <= 10)));
            (splineData.data).SkipLast(1).Zip((splineData.data).Skip(1)).Should().OnlyContain(t =>
                        Math.Abs(t.Second.x - t.First.x - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
        }
    }
}
