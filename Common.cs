using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLemmix4
{
	public static class extMethod
  {
    public static float Floor(this float f) => (float)Math.Floor(f);
    public static float Round(this float f) => (float)Math.Round(f);
    public static float Ceil(this float f) => (float)Math.Ceiling(f);

    public static Int32 MarshalInt32(this IntPtr ptr)
    {
      int r = Marshal.ReadInt32(ptr);



      return r;

    }

    public static void free(this IntPtr ptr)
    {
      Marshal.FreeHGlobal(ptr);
    }

    public static T CopyProperties<T>(this T i)
    {
      var tType = typeof(T);
      var r = Activator.CreateInstance(tType);

      foreach (var x in tType.GetProperties())
      {
        x.SetValue(x.GetValue(i), r);
      }

      return (T)r;


    }

    public static double DistanceFrom(this Point p1, Point p2)
    {

      double distance = Math.Round(Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2)), 1);
      return distance;
    }

    public static bool getBit(byte b, int num)
    {
      return (b & (1 << num)) != 0;
    }
    public static bool getBit(short b, int num)
    {
      return (b & (1 << num)) != 0;

    }


  }
  public static class ArrayExtentions
  {
    public static Array ResizeArray(this Array arr, int[] newSizes)
    {
      if (newSizes.Length != arr.Rank)
      {
        throw new ArgumentException("arr must have the same number of dimensions as there are elements in newSizes", "newSizes");
      }

      var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
      var sizesToCopy = new int[newSizes.Length];
      for (var i = 0; i < sizesToCopy.Length; i++)
      {
        sizesToCopy[i] = Math.Min(newSizes[i], arr.GetLength(i));
      }

      var currentPositions = new int[sizesToCopy.Length];
      CopyArray(arr, temp, sizesToCopy, currentPositions, 0);

      return temp;
    }

    private static void CopyArray(Array arr, Array temp, int[] sizesToCopy, int[] currentPositions, int dimmension)
    {
      if (arr.Rank - 1 == dimmension)
      {
        //Copy this Array
        for (var i = 0; i < sizesToCopy[dimmension]; i++)
        {
          currentPositions[dimmension] = i;
          temp.SetValue(arr.GetValue(currentPositions), currentPositions);
        }
      }
      else
      {
        //Recursion one dimmension higher
        for (var i = 0; i < sizesToCopy[dimmension]; i++)
        {
          currentPositions[dimmension] = i;
          CopyArray(arr, temp, sizesToCopy, currentPositions, dimmension + 1);
        }
      }
    }
  }
  public static class Common
  {

    public static int strncmp(char[] a, char[] b, int size)
    {

      return ComputeGenericDistance<char>(a.Take(size).ToArray(), b.Take(size).ToArray());



    }



    /// <summary>
    /// Compute the distance between two strings.
    /// </summary>
    /// <param name=s>The first of the two strings.</param>
    /// <param name=t>The second of the two strings.</param>
    /// <returns>The Levenshtein cost.</returns>
    public static int Compute(string s, string t)
    {
      int n = s.Length;
      int m = t.Length;
      int[,] d = new int[n + 1, m + 1];

      // Step 1
      if (n == 0)
      {
        return m;
      }

      if (m == 0)
      {
        return n;
      }

      // Step 2
      for (int i = 0; i <= n; d[i, 0] = i++)
      {
      }

      for (int j = 0; j <= m; d[0, j] = j++)
      {
      }

      // Step 3
      for (int i = 1; i <= n; i++)
      {
        //Step 4
        for (int j = 1; j <= m; j++)
        {
          // Step 5
          int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

          // Step 6
          d[i, j] = Math.Min(
              Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
              d[i - 1, j - 1] + cost);
        }
      }
      // Step 7
      return d[n, m];
    }
    /// <summary>
    /// computes distance between two T1s
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="s">first T1</param>
    /// <param name="t">second T1</param>
    /// <returns></returns>
    public static int ComputeGenericDistance<T1>(T1[] s, T1[] t)
    {
      int n = s.Length;
      int m = t.Length;
      int[,] d = new int[n + 1, m + 1];

      // Step 1
      if (n == 0)
      {
        return m;
      }

      if (m == 0)
      {
        return n;
      }

      // Step 2
      for (int i = 0; i <= n; d[i, 0] = i++)
      {
      }

      for (int j = 0; j <= m; d[0, j] = j++)
      {
      }

      // Step 3
      for (int i = 1; i <= n; i++)
      {
        //Step 4
        for (int j = 1; j <= m; j++)
        {
          // Step 5
          int cost = (t[j - 1].Equals(s[i - 1])) ? 0 : 1;


          // Step 6
          d[i, j] = Math.Min(
              Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
              d[i - 1, j - 1] + cost);
        }
      }
      // Step 7
      return d[n, m];
    }


    public static object Eval(string sCSCode, Form parent)
    {

      CSharpCodeProvider c = new CSharpCodeProvider();
      ICodeCompiler icc = c.CreateCompiler();
      CompilerParameters cp = new CompilerParameters();

      cp.ReferencedAssemblies.Add("system.dll");
      cp.ReferencedAssemblies.Add("system.xml.dll");
      cp.ReferencedAssemblies.Add("system.data.dll");
      cp.ReferencedAssemblies.Add("system.windows.forms.dll");
      cp.ReferencedAssemblies.Add("system.drawing.dll");

      //cp.CompilerOptions = "/t:library";
      cp.GenerateInMemory = true;
      //cp.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().FullName);
      cp.ReferencedAssemblies.Add(typeof(Program).Assembly.Location);
      StringBuilder sb = new StringBuilder("");
      sb.Append("using System;\n");
      sb.Append("using System.Xml;\n");
      sb.Append("using System.Data;\n");
      sb.Append("using System.Data.SqlClient;\n");
      sb.Append("using System.Windows.Forms;\n");
      sb.Append("using System.Drawing;\n");

      sb.Append("namespace CSCodeEvaler{ \n");
      sb.Append("public class CSCodeEvaler{ \n");
      sb.Append("public object EvalCode(Form frm){\n");
      sb.Append(sCSCode);
      sb.AppendLine("return null;");
      sb.Append("} \n");
      sb.Append("} \n");
      sb.Append("}\n");

      CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
      if (cr.Errors.Count > 0)
      {
        /* MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText,
            "Error evaluating cs code", MessageBoxButtons.OK,
            MessageBoxIcon.Error);*/

        StringBuilder sbe = new StringBuilder();
        foreach (var er in cr.Errors)
        {
          sbe.Append((er as CompilerError).ErrorText);
        }

        return sbe.ToString();


      }

      System.Reflection.Assembly a = cr.CompiledAssembly;

      object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

      Type t = o.GetType();
      MethodInfo mi = t.GetMethod("EvalCode");

      object s = mi.Invoke(o, new object[] { parent });
      return s;

    }
    public static byte[] scale(byte[] arr, int w, int h, float scale, out int newW, out int newH)
    {



      newW = (int)Math.Floor(w * scale);
      newH = (int)Math.Floor(h * scale);
      int len = newW * newH;
      byte[] r = new byte[len];

      for (float y = 0; y < newH; y++)
        for (float x = 0; x < newW; x++)
        {
          int srcX = (int)Math.Round((x / newW * w));
          int srcY = (int)Math.Round((y / newH * h));
          srcX = Math.Min(srcX, w - 1);
          srcY = Math.Min(srcY, h - 1);
          byte src = arr[srcX + srcY * w];

          r[(int)x + (int)y * newW] = src;
        }

      return r;


    }

  }
  public static class SizeHelper
  {
    private static Dictionary<Type, int> sizes = new Dictionary<Type, int>();

    public static int SizeOf(Type type)
    {
      int size;
      if (sizes.TryGetValue(type, out size))
      {
        return size;
      }

      size = SizeOfType(type);
      sizes.Add(type, size);
      return size;
    }

    private static int SizeOfType(Type type)
    {
      var dm = new DynamicMethod("SizeOfType", typeof(int), new Type[] { });
      ILGenerator il = dm.GetILGenerator();
      il.Emit(OpCodes.Sizeof, type);
      il.Emit(OpCodes.Ret);
      return (int)dm.Invoke(null, null);
    }
  }
}
