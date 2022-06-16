using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CLemmix4.Lemmix.Utils
{
	public class Parser<T>  where T : class
	{

		public string Source { get; set; }
		public string[] Lines { get; }
		public Parser(string source)
		{
			Source = source;
			Lines = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		}
		static Regex rgOneline = new Regex(@"^\s{0,}(?<name>[A-Z]+)\s{1,}(?<item>.*?)$");
		static Regex rgObject = new Regex(@"^\s{0,}\$(?<obj>[A-Z]+)$");
		static Regex rgObjectEnd = new Regex(@"^\s{0,}\$END");

 

		public void Parse(T obj)
		{
			PropertyInfo[] infs = obj.GetType().GetProperties();

		//	foreach (var l in Lines)
		for(int i= 0; i < Lines.Count(); i++)
			{
				var l = Lines[i];
				if (rgOneline.IsMatch(l))
				{

					Match m = rgOneline.Match(l);
					string rName = m.Groups["name"].Value;
					string rItem = m.Groups["item"].Value;

					var inf = infs.FirstOrDefault(o => o.Name.ToUpper() == rName);

					if (inf != null)
					{
						if (inf.isList())
						{
							Type itemType = inf.PropertyType.GetGenericArguments()[0];

							object List1 = inf.GetValue(obj);
							if (List1 == null)
							{
								List1 = Activator.CreateInstance(inf.PropertyType);
								inf.SetValue(obj, List1);
							}
							var List1Add = inf.PropertyType.GetMethod("Add");
							if (List1Add != null)
							{ 
									List1Add.Invoke(List1, new object[] { rItem });

							}
						}
						else {
							if (inf.PropertyType == typeof(string))
							{
								inf.SetValue(obj, (string)rItem);
							}
							else if (inf.PropertyType == typeof(int))
							{
								int num = -1;
								if (int.TryParse(rItem, out num))
								{
									inf.SetValue(obj, num);

								}
							}
						}


					}


				}
				else if (rgObject.IsMatch(l))
				{ 
				
					Match objMatch = rgObject.Match(l);

					string propToOpen = objMatch.Groups["obj"].Value;

					PropertyInfo inf = obj.GetType().GetProperties().FirstOrDefault(o => o.Name.ToUpper() == propToOpen);



					if (inf.isList())
					{

						Type itemType = inf.PropertyType.GetGenericArguments()[0];

						object List1 = inf.GetValue(obj);
						if (List1 == null)
						{
							List1 = Activator.CreateInstance(inf.PropertyType);
							inf.SetValue(obj, List1);
						}

						var List1Add = inf.PropertyType.GetMethod("Add");
						if (List1Add != null)
						{
							var newItem = Activator.CreateInstance(itemType);
							List1Add.Invoke(List1, new object[] { newItem });
							while (true)
							{
								i++;
								if (i > Lines.Count()) break;
								l = Lines[i];
								if (rgObjectEnd.IsMatch(l))
								{
									break;
								}

								if (rgOneline.IsMatch(l))
								{
									Match oneLineMatch = rgOneline.Match(l);
									string rName = oneLineMatch.Groups["name"].Value;
									string rItem = oneLineMatch.Groups["item"].Value;

									var innerInf = itemType.GetProperties().FirstOrDefault(o => o.Name.ToUpper() == rName);

									if (innerInf.isList())
									{

										object List2 = innerInf.GetValue(newItem);
										if (List2 == null)
										{
											List2 = Activator.CreateInstance(innerInf.PropertyType);
											innerInf.SetValue(newItem, List2);
										}

										var List2Add = innerInf.PropertyType.GetMethod("Add");

										if (innerInf.innerTypeOf(typeof(string)))
										{
											List2Add.Invoke(List2, new object[] { rItem });
										}
									}
									else {
										if (innerInf != null)
										{
											if (innerInf.PropertyType == typeof(string))
											{
												innerInf.SetValue(obj, (string)rItem);
											}
											else if (innerInf.PropertyType == typeof(int))
											{
												int num = -1;
												if (int.TryParse(rItem, out num))
												{
													innerInf.SetValue(obj, num);

												}
											}

										}
									}

								}
							}
						}
					}
					else { //not a list of an object, just an object!

						var nobj = inf.GetValue(obj);
						if (nobj == null)
						{
							nobj = Activator.CreateInstance(inf.PropertyType);
							inf.SetValue(obj, nobj);
						}

						while (true)
						{
							i++;
							if (i > Lines.Count()) break;
							l = Lines[i];
							if (rgObjectEnd.IsMatch(l))
							{
								break;
							}
							if (rgOneline.IsMatch(l))
							{
								Match oneLineMatch = rgOneline.Match(l);
								string rName = oneLineMatch.Groups["name"].Value;
								string rItem = oneLineMatch.Groups["item"].Value;
								PropertyInfo innerInf = nobj.GetType().GetProperties()
									.FirstOrDefault(o => o.Name.ToUpper() == rName);
								if (innerInf.isList())
								{
						Type itemType = innerInf.PropertyType.GetGenericArguments()[0];
									object List1 = innerInf.GetValue(nobj);
									if (List1 == null)
									{
										List1 = Activator.CreateInstance(innerInf.PropertyType);
										innerInf.SetValue(nobj, List1);
									}

									var List1Add = innerInf.PropertyType.GetMethod("Add");
									List1Add.Invoke(List1, new object[] { rItem });
								}
								else {
									if (innerInf != null)
									{
										if (innerInf.PropertyType == typeof(string))
										{
											innerInf.SetValue(obj, (string)rItem);
										}
										else if (innerInf.PropertyType == typeof(int))
										{
											int num = -1;
											if (int.TryParse(rItem, out num))
											{
												innerInf.SetValue(obj, num);

											}
										}

									}
								}
							}

						}

					}

				}

			}


		}

	}

	 
}
