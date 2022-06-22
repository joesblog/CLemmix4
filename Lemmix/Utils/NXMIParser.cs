using CLemmix4.Lemmix.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Utils
{
	public class NXMIParser<T> : IDisposable where T : abParsable<T>
	{

		public abParsable<T> MainItem { get; set; }

		public string src { get; set; }

		public Tokenizer tkz { get; set; }

		public TKStream str { get; set; }
		public NXMIParser(T obj)
		{
			this.MainItem = obj;
			this.src = ((abParsable<T>)obj).source;

			if (src.isNotNullWS())
			{
				tkz = new Tokenizer(src);
				str = new TKStream(tkz.tokenize());
			}
		}

		public void Parse()
		{
			Token c = str.peek();


			Token.enmTokenKind lastKind = Token.enmTokenKind.EOL;
			PropertyInfo mainCurProp = null;
			Type maintype = typeof(T);
			while (true)
			{

				if (c == null || c.tKind.HasFlag(Token.enmTokenKind.EOF))
				{
					break;
				}

				if (c.tKind.HasFlag(Token.enmTokenKind.EOL))
				{
					c = str.read();
					continue;
				}

				if (c.tKind.HasFlag(Token.enmTokenKind.PROPERTYID) && mainCurProp == null)
				{
					mainCurProp = maintype.GetProperties().FirstOrDefault(o => o.Name.ToUpper() == c.value);

					Enum mainCurFlag = null;

					if (mainCurProp == null)
					{
						if (typeof(iFlaggable<>).IsAssignableFrom(MainItem.GetType()))
						{ 
						
						}
					}

					if (mainCurProp == null)
					{
						c = str.read();
						continue;
					}
					bool propIsList = mainCurProp.isList();
					bool propIsString = mainCurProp.PropertyType == typeof(string);
					bool propIsNumber = mainCurProp.PropertyType == typeof(int);
					bool propIsBool = mainCurProp.PropertyType == typeof(bool);
					bool propIsAffixedInt = mainCurProp.PropertyType == typeof(AffixedInteger);
					if (propIsString)
					{
						string val = "";
						while (true)
						{
							if (c.tKind.HasFlag(Token.enmTokenKind.EOL))
							{
								c = str.read(); break;
							};
							c = str.read();
							val += $"{c.value}{c.whitespaceafter}";
						}
						val = val.Trim();

						mainCurProp.SetValue(MainItem, val);
						mainCurProp = null;
						continue;
					}
					else if (propIsBool)
					{
						mainCurProp.SetValue(MainItem, true);
						mainCurProp = null;

						c = str.read();
						continue;
					}
					else if (propIsNumber)
					{
						c = str.read();
						int pnm = 0;
						if (int.TryParse(c.value, out pnm))
						{
							mainCurProp.SetValue(MainItem, pnm);
						}
						mainCurProp = null;

						c = str.read();
						continue;
					}
					else if (propIsAffixedInt)
					{
						mainCurProp.SetValue(MainItem, new AffixedInteger(c));
						c = str.read();
						continue;
					}
					else if (propIsList)
					{
						object objSubObject = null;
						MethodInfo subObjectAddMethod = null;
						bool objCreatedOrExisted = handleObjectInstantiation(mainCurProp, MainItem, out objSubObject, true, out subObjectAddMethod); //create the object whether it a reg obj or a List<obj>
						Type listOfType = mainCurProp.PropertyType.GetGenericArguments()[0];
						if (objSubObject != null && subObjectAddMethod != null && objCreatedOrExisted)
							if (listOfType == typeof(string))
							{
								string val = "";
								while (true)
								{
									if (c.tKind.HasFlag(Token.enmTokenKind.EOL))
									{
										c = str.read(); break;
									};
									c = str.read();
									val += $"{c.value}{c.whitespaceafter}";
								}
								val = val.Trim();

								subObjectAddMethod?.Invoke(objSubObject, new object[] { val });
								mainCurProp = null;
								continue;
							}

					}

				}//ToDo handle list of base types string, int, bool etc.. 
				if (c.tKind.HasFlag(Token.enmTokenKind.OBJECT_OPEN))
				{
					PropertyInfo objProp = maintype.GetProperties().FirstOrDefault(o => "$" + o.Name.ToUpper() == c.value);
					if (objProp == null) return;

					bool objPropIsList = objProp.isList();
					bool objPropIsDict = objProp.isDictionary();

					var openPos = str.pos;
					Token endObj = null;
					var posend = str.huntKind(Token.enmTokenKind.OBJECT_CLOSE, out endObj);


					if (!posend.HasValue) return;



					TKStream ostr = null;
					if (posend.HasValue)
					{
						/*var tks = str.ReadGrab(posend.Value - openPos);
						ostr = new TKStream(tks.ToList());*/

						ostr = new TKStream(str.ReadGrabRange(openPos, posend.Value));
						c = str.read();
					}
					if (ostr == null) return;


					object objSubObject = null;
					MethodInfo subObjectAddMethod = null;
					bool objCreatedOrExisted = handleObjectInstantiation(objProp, MainItem, out objSubObject, objPropIsList, out subObjectAddMethod); //create the object whether it a reg obj or a List<obj>

					if (objPropIsList && subObjectAddMethod != null && objSubObject != null)
					{
						object subObjAdded = null;
						//lets get the inner type
						Type innerType = objProp.PropertyType.GetGenericArguments()[0];

						//create a new instance 
						var InnerItem = Activator.CreateInstance(innerType);

						//add it via the addmethod
						subObjectAddMethod.Invoke(objSubObject, new object[] { InnerItem });


						//process inner objects
						Token ic = ostr.peek();
						if (ic.tKind == Token.enmTokenKind.OBJECT_OPEN) ic = ostr.read();
						while (true)
						{

							if (ic == null) break;

							if (ic.tKind.HasFlag(Token.enmTokenKind.EOL) || ic.tKind.HasFlag(Token.enmTokenKind.OBJECT_OPEN))
							{
								ic = ostr.read(); continue;
							}

							if (ic.tKind.HasFlag(Token.enmTokenKind.PROPERTYID))
							{
								var innerSubProp = innerType.GetProperties().FirstOrDefault(o => o.Name.ToUpper() == ic.value);

								if (innerSubProp == null)
								{
									if (innerType.ImplementsGenericInterface(typeof(iFlaggable<>)))
									{
										var cst = innerType.GetProperty("Flags");
										var vlu = cst.GetValue(InnerItem);
										if (vlu == null) vlu = Activator.CreateInstance(cst.PropertyType);


										int enumInt = (int)vlu;
										bool set = false;
										if (vlu is System.Enum)
										{
											foreach (var i in Enum.GetValues(vlu.GetType()))
											{
												int a = (int)i;
												string s = i.ToString();
												if (s.ToUpper() == ic.value)
												{
													enumInt |= a;
													break;
												}
											}
										}
										cst.SetValue(InnerItem, enumInt);

									}
									ic = ostr.read();
									continue;
								}

								if (innerSubProp == null)
								{
									ic = ostr.read();
									continue;
								};
								bool propIsList = innerSubProp.isList();
								bool propIsString = innerSubProp.PropertyType == typeof(string);
								bool propIsNumber = innerSubProp.PropertyType == typeof(int);
								bool propIsBool = innerSubProp.PropertyType == typeof(bool);
								bool propIsAffixedInt = innerSubProp.PropertyType == typeof(AffixedInteger);

								if (propIsList)
								{
									object objInnerList = null;
									MethodInfo objInnerAddMethod = null;
									handleObjectInstantiation(innerSubProp, InnerItem, out objInnerList, true, out objInnerAddMethod);
									Type innerSupPropType = innerSubProp.PropertyType.GetGenericArguments()[0];
									bool innerpropIsString = innerSupPropType == typeof(string);
									bool innerpropIsNumber = innerSupPropType == typeof(int);

									if (objInnerList != null)
									{
										if (innerpropIsString)
										{
											string val = "";



											while (true)
											{
												if (ic.tKind.HasFlag(Token.enmTokenKind.EOL))
												{
													ic = ostr.read(); break;
												};
												ic = ostr.read();
												val += $"{ic.value}{ic.whitespaceafter}";
											}
											val = val.Trim();



											objInnerAddMethod?.Invoke(objInnerList, new object[] { val });

										}
									}


								}
								else if (propIsString)
								{
									string val = "";
									while (true)
									{
										if (ic.tKind.HasFlag(Token.enmTokenKind.EOL))
										{
											ic = ostr.read(); break;
										};
										ic = ostr.read();
										val += $"{ic.value}{ic.whitespaceafter}";
									}
									val = val.Trim();

									innerSubProp.SetValue(InnerItem, val);
									innerSubProp = null;
								}
								else if (propIsBool)
								{
									innerSubProp.SetValue(InnerItem, true);
									ic = ostr.read();
								}
								else if (propIsAffixedInt)
								{
									ic = ostr.read();

									innerSubProp.SetValue(InnerItem, new AffixedInteger(ic));
									ic = ostr.read();

								}
								else if (propIsNumber)
								{
									ic = ostr.read();
									int pnm = 0;
									if (int.TryParse(ic.value, out pnm))
									{
										innerSubProp.SetValue(InnerItem, pnm);
									}

								}
								continue;

							}


							ic = ostr.read();
						}

					}
					else if (!objPropIsList &&  !objPropIsDict && objSubObject != null)
					{

						Type innerType = objSubObject.GetType();

						Token ic = ostr.peek();
						if (ic.tKind == Token.enmTokenKind.OBJECT_OPEN) ic = ostr.read();
						while (true)
						{

							if (ic == null) break;

							if (ic.tKind.HasFlag(Token.enmTokenKind.EOL) || ic.tKind.HasFlag(Token.enmTokenKind.OBJECT_OPEN))
							{
								ic = ostr.read(); continue;
							}

							if (ic.tKind.HasFlag(Token.enmTokenKind.PROPERTYID))
							{
								var innerSubProp = innerType.GetProperties().FirstOrDefault(o => o.Name.ToUpper() == ic.value);

								if (innerSubProp == null)
								{
									if (innerType.ImplementsGenericInterface(typeof(iFlaggable<>)))
									{
										var cst = innerType.GetProperty("Flags");
										var vlu = cst.GetValue(objSubObject);
										if (vlu == null) vlu = Activator.CreateInstance(cst.PropertyType);


										int enumInt = (int)vlu;
										bool set = false;
										if (vlu is System.Enum)
										{
											foreach (var i in Enum.GetValues(vlu.GetType()))
											{
												int a = (int)i;
												string s = i.ToString();
												if (s.ToUpper() == ic.value)
												{
													enumInt |= a;
													break;
												}
											}
										}
										cst.SetValue(objSubObject, enumInt);

									}
									ic = ostr.read();
									continue;
								}

								if (innerSubProp == null)
								{
									ic = ostr.read();
									continue;
								};
								bool propIsList = innerSubProp.isList();
								bool propIsString = innerSubProp.PropertyType == typeof(string);
								bool propIsNumber = innerSubProp.PropertyType == typeof(int);
								bool propIsBool = innerSubProp.PropertyType == typeof(bool);
								bool propIsAffixedInt = innerSubProp.PropertyType == typeof(AffixedInteger);

								if (propIsList)
								{
									object objInnerList = null;
									MethodInfo objInnerAddMethod = null;
									handleObjectInstantiation(innerSubProp, objSubObject, out objInnerList, true, out objInnerAddMethod);
									Type innerSupPropType = innerSubProp.PropertyType.GetGenericArguments()[0];
									bool innerpropIsString = innerSupPropType == typeof(string);
									bool innerpropIsNumber = innerSupPropType == typeof(int);

									if (objInnerList != null)
									{
										if (innerpropIsString)
										{
											string val = "";



											while (true)
											{
												if (ic.tKind.HasFlag(Token.enmTokenKind.EOL))
												{
													ic = ostr.read(); break;
												};
												ic = ostr.read();
												val += $"{ic.value}{ic.whitespaceafter}";
											}
											val = val.Trim();



											objInnerAddMethod?.Invoke(objInnerList, new object[] { val });

										}
									}


								}
								else if (propIsString)
								{
									string val = "";
									while (true)
									{
										if (ic.tKind.HasFlag(Token.enmTokenKind.EOL))
										{
											ic = ostr.read(); break;
										};
										ic = ostr.read();
										val += $"{ic.value}{ic.whitespaceafter}";
									}
									val = val.Trim();

									innerSubProp.SetValue(objSubObject, val);
									innerSubProp = null;
								}
								else if (propIsBool)
								{
									innerSubProp.SetValue(objSubObject, true);
									ic = ostr.read();
								}
								else if (propIsAffixedInt)
								{
									ic = ostr.read();

									innerSubProp.SetValue(objSubObject, new AffixedInteger(ic));
									ic = ostr.read();

								}
								else if (propIsNumber)
								{
									ic = ostr.read();
									int pnm = 0;
									if (int.TryParse(ic.value, out pnm))
									{
										innerSubProp.SetValue(objSubObject, pnm);
									}

								}
								continue;

							}


							ic = ostr.read();
						}

					}

				}

				c = str.read();



			}
		}

		private bool handleObjectInstantiation(PropertyInfo objProp, object mainObj, out object subObj, bool propIsList, out MethodInfo addMethod)
		{
			object r = objProp.GetValue(mainObj);
			if (r != null)
			{
				subObj = r;
				if (propIsList)
					addMethod = objProp.PropertyType.GetMethod("Add");
				else addMethod = null;
				return true;
			}
			else
			{
				subObj = Activator.CreateInstance(objProp.PropertyType);

				if (propIsList)
				{
					addMethod = objProp.PropertyType.GetMethod("Add");
				}
				else
				{
					addMethod = null;
				}

				if (subObj != null)
				{
					objProp.SetValue(mainObj, subObj);
					return true;
				}



			}

			return false;

		}

		public void Dispose()
		{
			//Todo
		}
	}
}
