using System;
using FluentAssertions;
using System.Linq;
using Xunit;
using ViewModel;
using Moq;

namespace Test_ViewModel
{
    public class Test_VM
    {
        public double leftEnd = 1;
        public double rightEnd = 10;
        public int nRawNodes = 10;
        public bool uni = true;
        public int nGrid = 10;
        public double leftdif = 0;
        public double rightdif = 1;
        public int fRaw_num = 0;
        public ViewData viewData;

        private void init_vd()
        {
            viewData.leftEnd = leftEnd;
            viewData.rightEnd = rightEnd;
            viewData.nRawNodes = nRawNodes;
            viewData.uni = uni;
            viewData.nGrid = nGrid;
            viewData.leftdif = leftdif;
            viewData.rightdif = rightdif;
            viewData.fRaw = viewData.listFRaw[fRaw_num];
        }

        [Fact]
        public void BasicTest()
        {
            leftdif = 0;
            rightdif = 0;
            var er = new Mock<IUIServices>();
            viewData = new ViewData(er.Object);
            init_vd();
            viewData.RunCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);
            (viewData.rawData.x).SkipLast(1).Zip((viewData.rawData.x).Skip(1)).Should().OnlyContain(t => t.First < t.Second);
            (viewData.rawData.x).Zip((viewData.rawData.y)).Should().OnlyContain(t => Math.Abs(t.First - t.Second) < 1e-10);
            (viewData.splineData.data).SkipLast(1).Zip((viewData.splineData.data).Skip(1)).Should().OnlyContain(t =>
                        Math.Abs(t.Second.x - t.First.x - (rightEnd - leftEnd) / (nGrid - 1)) < 1e-10);
            (viewData.splineData.data).Zip((viewData.splineData.data)).Should().OnlyContain(t => Math.Abs(t.First.x - t.Second.y) < 1e-10);
            (viewData.splineData.integral).Should().Be(rightEnd * rightEnd / 2 - leftEnd * leftEnd / 2);
        }
        [Fact]
        public void LimitsErrorTest()
        {
            leftdif = 0;
            rightdif = 0;
            leftEnd = 10;
            rightEnd = 1;
            var er = new Mock<IUIServices>();
            viewData = new ViewData(er.Object);
            init_vd();
            viewData.RunCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void nGridnumErrorTest()
        {
            leftdif = 0;
            rightdif = 0;
            nGrid = 1;
            var er = new Mock<IUIServices>();
            viewData = new ViewData(er.Object);
            init_vd();
            viewData.RunCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void nRawnumErrorTest()
        {
            leftdif = 0;
            rightdif = 0;
            nRawNodes = -1;
            var er = new Mock<IUIServices>();
            viewData = new ViewData(er.Object);
            init_vd();
            viewData.RunCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Once);
        }
    }
}
