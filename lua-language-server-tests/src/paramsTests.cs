using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace src
{
    
    public class ParamsTests
    {
        [Fact]
        void BoolParamParse() {
            param.Params.instance.parse("--bool");
        }
        [Fact]
        void BoolParamCount()
        {
            param.Params.instance.parse("--bool2");
            Assert.Equal(1, param.Params.instance.Lenght);
        }
        [Fact]
        void BoolParamGetTrue(){
            param.Params.instance.parse("--bool3");
            Assert.Equal("true", param.Params.instance["--bool3"]);
        }
        [Fact]
        void BoolParamGetFalse()
        {
               Assert.Equal("true", param.Params.instance["--bool"]);
        }
        [Fact]
        void ParamValue(){
            param.Params.instance.parse("--test=value");
            Assert.Equal("value", param.Params.instance["--test"]);
        }
    }
}
