using System;

namespace TimeLibrary {
  
  public class Demo {
    
    // This same code is called by an xUnit test and by a console app.
    // In the xUnit case, the argument invokes Xunit.Assert.Equal().
    // In the console app case, the argument is null.
    // See example output at end.
    public void run(Action<object, object> assertEqual) {
      this.assertEqual = assertEqual;
      wl();
      DateTimeDebug.FakeDateTime = DateTime.Today + TimeSpan.FromSeconds(10);
      /*
         Timing diagram
         
         real time   10   11   12   13   14   15   16   17   18   19   20   21   22   23   24  
         now         10   11   11   11   12   13   13   10   11  *10*  11   10   10  *15*  15  
         lag          0    0    1    2    2    2    3  * 7*   7    9    9  *11*  12    8    9  
         running     ++++++----------++++++++++-----++++++++++++++++++++--------------------++
         nowSaved   ( 0)  11   11  (11) (11)  13  (13) (13) (13) (13)  11  (10)  10  (15) (15)
         lagSaved     0  ( 0) ( 0)   2    2  ( 2)   3  ( 7)   7  ( 9) ( 9) ( 9) ( 9) ( 9)   9  
      */
      step(true,    null,   null, dt(10), ts( 0), 0);
      step(false,   null,   null, dt(11), ts( 0), 1);
      step(null,    null,   null, dt(11), ts( 1), 1);
      step(true,    null,   null, dt(11), ts( 2), 1);
      step(null,    null,   null, dt(12), ts( 2), 1);
      step(false,   null,   null, dt(13), ts( 2), 1);
      step(true,    null,   null, dt(13), ts( 3), 1);
      step(null,    null,  ts(7), dt(10), ts( 7), 1);
      step(null,    null,   null, dt(11), ts( 7), 1);
      step(null,  dt(10),   null, dt(10), ts( 9), 1);
      step(false,   null,   null, dt(11), ts( 9), 1);
      step(null,    null, ts(11), dt(10), ts(11), 1);
      step(null,    null,   null, dt(10), ts(12), 1);
      step(null,  dt(15),   null, dt(15), ts( 8), 1);
      step(null,    null,   null, dt(15), ts( 9), 1);
      wl();
      if (problems != 0) wl($"Errors: {problems}");
    }

    int problems;
    
    DateTime dt(int secs) => DateTime.Today + TimeSpan.FromSeconds(secs);
    TimeSpan ts(int secs) =>                  TimeSpan.FromSeconds(secs);

    private Action<object, object> assertEqual;
    void w (object o)        { if (assertEqual == null)  Console.Write    (o); }
    void wl(object o = null) { if (assertEqual == null)  Console.WriteLine(o); }

    void step(bool? running, DateTime? now, TimeSpan? lag, DateTime nowExpect, TimeSpan lagExpect, int delay) {
      if (delay > 0) {
        w($"   Sleep {delay}s");
        DateTimeDebug.FakeDateTime += TimeSpan.FromSeconds(delay);
      }
      wl();
      if (now == null && lag == null) {                   w("            "); }
      else {
        if (now != null) { DateTimeDebug.Now = now.Value; w($"Now -> {now.Value.Second ,2}   "); }
        if (lag != null) { DateTimeDebug.Lag = lag.Value; w($"Lag -> {lag.Value.Seconds,2}   "); }
      }
      status(nowExpect, lagExpect);
      if (running != null && running != DateTimeDebug.Running) {
        w($"Running -> {running,-5}");
        DateTimeDebug.Running = running.Value;
      } else {
        w($"Running is {DateTimeDebug.Running,-5}");
      }
    }

    void status(DateTime nowExpect, TimeSpan lagExpect) {
      doAssert(DateTimeDebug.Now, nowExpect);
      doAssert(DateTimeDebug.Lag, lagExpect);
      w($"Lag: {s(DateTimeDebug.Lag.Seconds, lagExpect.Seconds)} ");
      w($"Now: {s(DateTimeDebug.Now.Second,  nowExpect.Second )} ");
      w($"Real: { DateTimeDebug.SourceDateTime.Second,2}   ");
      return;

      void doAssert(object actual, object expected) {
        if (assertEqual != null)  assertEqual(actual, expected);
      }

      string s(int actual, int expected) {
        string differ = "";
        if (actual != expected) {
          differ = $"<-(WRONG sh/be {expected})";
          ++problems;
        }
        return $"{actual,2}{differ}";
      }
    }
    
  }
}

/*
                             Example output

            Lag:  0   Now: 10   Real: 10   Running is True    Sleep 1s
            Lag:  0   Now: 11   Real: 11   Running -> False   Sleep 1s
            Lag:  1   Now: 11   Real: 12   Running is False   Sleep 1s
            Lag:  2   Now: 11   Real: 13   Running -> True    Sleep 1s
            Lag:  2   Now: 12   Real: 14   Running is True    Sleep 1s
            Lag:  2   Now: 13   Real: 15   Running -> False   Sleep 1s
            Lag:  3   Now: 13   Real: 16   Running -> True    Sleep 1s
Lag ->  7   Lag:  7   Now: 10   Real: 17   Running is True    Sleep 1s
            Lag:  7   Now: 11   Real: 18   Running is True    Sleep 1s
Now -> 10   Lag:  9   Now: 10   Real: 19   Running is True    Sleep 1s
            Lag:  9   Now: 11   Real: 20   Running -> False   Sleep 1s
Lag -> 11   Lag: 11   Now: 10   Real: 21   Running is False   Sleep 1s
            Lag: 12   Now: 10   Real: 22   Running is False   Sleep 1s
Now -> 15   Lag:  8   Now: 15   Real: 23   Running is False   Sleep 1s
            Lag:  9   Now: 15   Real: 24   Running is False
            
*/