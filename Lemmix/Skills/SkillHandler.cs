using CLemmix4.Lemmix.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Skills
{

	public abstract class absSkill
	{
		public virtual SpriteDefinition SpriteDef { get; set; } = null;

		public virtual absSkill[] NotAssignableFrom { get; set; } = null;
		public virtual absSkill[] OnlyAssignableFrom { get; set; } = null;

		public virtual string Name { get; } = "UNDEFINED";

		public bool MayAssign(Lemming L)
		{
			return true;
		}

		public virtual void Handle(Lemming L)
		{ 
		
		}

		public void Transition(Lemming L, bool DoTurn = false)
		{ 
		
		}

	
		
	}

	public class SkillHandler
	{

		public static Dictionary<int, absSkill> lupIdSkill = InitSkillLookup();
		public static Dictionary<absSkill, int> lupSkillId;
		public static Dictionary<string, int> lupSkillNameId;


		public static List<absSkill> allSkills
		{
			get {
				return lupSkillId.Select(o => o.Key).ToList();
			}
		}
		private static Dictionary<int, absSkill> InitSkillLookup()
		{
			var abs = typeof(absSkill)
				 .Assembly.GetTypes()
				 .Where(o => o.IsSubclassOf(typeof(absSkill)) && !o.IsAbstract)
				 .Select(o => (absSkill)Activator.CreateInstance(o)).ToArray();
			Dictionary<int, absSkill> r = new Dictionary<int, absSkill>();
			Dictionary<string, int> r2 = new Dictionary<string, int>();
			Dictionary<absSkill, int> r3 = new Dictionary<absSkill, int>();


			for (int i = 0; i < abs.Length; i++)
			{
				var c = abs[i];
				r.Add(i, c);
				r2.Add(c.Name, i);
				r3.Add(c, i);
			}

			lupSkillId = r3;
			lupSkillNameId = r2;
			return r;

		}
	}

}
