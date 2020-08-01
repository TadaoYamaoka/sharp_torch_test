using System;
using System.Collections.Generic;
using Python.Runtime;
using System.Runtime.InteropServices;

namespace sharp_torch_test
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            // You need to set the environment variable PYTHON_HOME.
            PythonEngine.PythonHome = Environment.ExpandEnvironmentVariables(@"%PYTHON_HOME%");
            PythonEngine.Initialize();
            InitPythonOutput();

            using (Py.GIL())
            {
                dynamic torch = Py.Import("torch");

                //var data = new float[] { 1.0f, 2.0f, 3.0f, 4.0f }; // error
                var data = new List<float> { 1.0f, 2.0f, 3.0f, 4.0f };
                var x = torch.as_tensor(data, dtype: torch.float32);
                Console.WriteLine(x);


                // see: https://github.com/pythonnet/pythonnet/issues/174
                dynamic sharp_torch_test = Py.Import("sharp_torch_test.test");
                var data2 = new float[] { 1.0f, 2.0f, 3.0f, 4.0f };
                var handler = GCHandle.Alloc(data2, GCHandleType.Pinned);
                var pointer = handler.AddrOfPinnedObject();
                sharp_torch_test.function((Int64)pointer, data2.Length);
                handler.Free();
                Console.WriteLine(string.Join(", ", data2));


                var data3 = new float[] { 1.0f, 2.0f, 3.0f, 4.0f };
                fixed (float* pointer3 = &data3[0])
                {
                    sharp_torch_test.function((Int64)pointer3, data3.Length);
                    Console.WriteLine(string.Join(", ", data3));
                }
            }
        }

        // see: https://github.com/pythonnet/pythonnet/issues/370
        private static void InitPythonOutput()
        {
            using (Py.GIL())
            {
                PythonEngine.RunSimpleString(
@"import sys
from System import Console
class output(object):
    def write(self, msg):
        Console.Write(msg)
    def writelines(self, msgs):
        for msg in msgs:
            Console.Write(msg)
    def flush(self):
        pass
    def close(self):
        pass
sys.stdout = sys.stderr = output()
");
            }
        }
    }
}
