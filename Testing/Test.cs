using Xunit;
using TimeLibrary;

namespace Testing {
  
  public class Test {
    
    [Fact]
    public void test() {
      new Demo().run(assertEqual);
      return;
      
      void assertEqual(object actual, object expected) {
        Assert.Equal(actual, expected);
      }
    }
    
  }

}