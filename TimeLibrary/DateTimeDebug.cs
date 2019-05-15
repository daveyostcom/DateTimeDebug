using System;

namespace TimeLibrary {

    /// <summary>
    /// <para>
    /// A pausable clock.
    /// </para><para>
    /// <c>DateTimeDebug</c> is useful for unit-testing code that uses <c>DateTime.Now</c>.
    /// The only changes required in the tested code are substitutions of <c>DateTime.Now</c>  
    /// with <c>DateTimeDebug.Now</c>.  
    /// Unit tests that use <c>DateTimeDebug.Now</c> are fast  
    /// because they simulate the passage of time without actually waiting,  
    /// and they are easier to write because they aren't subject to variations  
    /// in elapsed time caused by outside influences.
    /// </para><para>
    /// <c>DateTimeDebug</c> can also make interactive debugging easier.
    /// This use case requires using debugger hooks to cause <c>DateTimeDebug</c> to be paused  
    /// while the program being debugged is paused.
    /// Apparently there are no such debugger hooks for C# debugging as of this writing,
    /// but if you're using C# to build a simulation with its own debugger,
    /// your simulation can provide and use its own hooks.
    /// </para><para>
    /// Perhaps the features of <c>DateTimeDebug</c> could one day find their way into <c>DateTime</c>.
    /// </para><para>
    /// <c>DateTimeDebug</c> is not a data type; all <c>public</c> members are <c>static</c>.
    /// </para><para>
    /// 2019-05-11 Dave Yost
    /// http://github.com/daveyostcom/DateTimeDebug
    /// </para>
    /// </summary>
    public class DateTimeDebug {

    private DateTimeDebug() { }

    /*
       Timing diagram
       
       real time   10   11   12   13   14   15   16   17   18   19   20   21   22   23   24  
       now         10   11   11   11   12   13   13   10   11  *10*  11   10   10  *15*  15  
       lag          0    0    1    2    2    2    3  * 7*   7    9    9  *11*  12    8    9  
       running     ++++++----------++++++++++-----++++++++++++++++++++--------------------++
       nowSaved   ( 0)  11   11  (11) (11)  13  (13) (13) (13) (13)  11  (10)  10  (15) (15)
       lagSaved     0  ( 0) ( 0)   2    2  ( 2)   3  ( 7)   7  ( 9) ( 9) ( 9) ( 9) ( 9)   9  
    */

    /// <value>
    /// <para>
    /// <c>Now</c> is a <c>DateTime</c> value, initially identical to the value of <c>DateTime.Now</c>.
    /// possibly paused, possibly lagging behind <c>DateTime.Now</c>.
    /// </para><para>
    /// The behavior of <c>Now</c> can be influenced by the other properties.
    /// </para><para>
    /// Changing <c>Now</c> affects <c>Lag</c>.
    /// </para>
    /// </value>
    public static DateTime Now {
      get => Running
           ? SourceDateTime - lagWhenRunning
           : nowWhenPaused;
      set {
        if (Running)  lagWhenRunning = SourceDateTime - value;
        else          nowWhenPaused  = value;
      }
    }


    /// <value>
    /// <para>
    /// <c>Lag</c> is the amount of time that <c>Now</c> lags behind <c>DateTime.Now</c>.
    /// </para><para>
    /// <c>Lag</c> is <c>0</c> by default
    /// </para><para>
    /// The behavior of <c>Lag</c> can be influenced by the other properties.
    /// </para><para>
    /// Changing <c>Lag</c> affects <c>Now</c>.
    /// </para>
    /// </value>
    public static TimeSpan Lag {
      get => Running
           ? lagWhenRunning
           : SourceDateTime - nowWhenPaused;
      set {
        if (Running)  lagWhenRunning = value;
        else          nowWhenPaused  = SourceDateTime - value;
      }
    }

    
    /// <value>
    /// <para>
    /// <c>Running</c> is the running/paused state of <c>Now</c>.
    /// </para><para>
    /// <c>Running</c> is <c>true</c> by default.
    /// </para><para>
    /// Changing <c>Running</c> to <c>false</c> causes <c>Now</c> to pause and make no further progress.
    /// </para><para>
    /// Changing <c>Running</c> to <c>true</c> causes <c>Now</c> to resume, lagging behind <c>DateTime.Now</c>
    /// by the accumulated <c>Lag</c> caused by pausing.
    /// </para>
    /// </value>
    public static bool Running {
      get => _running;
      set {
        if (value == _running)  return;
        if (_running)  nowWhenPaused  = Now;
        else           lagWhenRunning = Lag;
        _running = value;
      }
    }
    private static bool _running = true;

    
    /// <summary>
    /// A <c>DateTime</c> to use instead of <c>DateTime.Now</c>. 
    /// </summary>
    public static DateTime? FakeDateTime { get; set; }

    
    /// <summary>
    /// The <c>DateTime</c> source for real time.
    /// </summary>
    /// <value>
    /// <para>
    /// <c>DateTime.Now</c> when <c>FakeDateTime == null</c>.
    /// </para><para>
    /// <c>FakeDateTime</c> when <c>FakeDateTime != null</c>.
    /// </para>
    /// </value>
    public static DateTime SourceDateTime => FakeDateTime != null
                                           ? FakeDateTime.Value
                                           : DateTime.Now;


    // Lag as of the last time Running went true.
    private static TimeSpan lagWhenRunning = TimeSpan.Zero;
    // Now as of the last time Running went false.
    private static DateTime nowWhenPaused;

  }
}