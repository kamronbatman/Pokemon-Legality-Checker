using System;

namespace LegalityChecker
{
	[Flags]
	public enum HT3 : byte
	{
		R			= 0x001,
		S			= 0x002,
		RS			= 0x003,
		E			= 0x004,
		RSE			= 0x007,
		Fr			= 0x008,
		Lg			= 0x010,
		FrLg			= 0x018,
		RSEFrLg		= 0x01F
	}

	[Flags]
	public enum HT4 : byte
	{
		D			= 0x001,
		P			= 0x002,
		DP			= 0x003,
		Pt			= 0x004,
		DPPt			= 0x007,
		HG			= 0x008,
		SS			= 0x010,
		HGSS			= 0x018,
		DPPtHGSS		= 0x01F
		B			= 0x020,
		W			= 0x040,
		BW			= 0x060
	}

	public enum GrowthType : byte
	{
		None,
		Cubic, //Medium-Fast - 1000000
		Erratic,
		Flux, //Flucuating
		Parabolic, //Medium-Slow
		Fast,
		Slow
	}

	public static class Utility
	{
		private static Stack<ConsoleColor> m_ConsoleColors = new Stack<ConsoleColor>();

		public static void PushColor( ConsoleColor color )
		{
			try
			{
				m_ConsoleColors.Push( Console.ForegroundColor );
				Console.ForegroundColor = color;
			}
			catch
			{
			}
		}

		public static void PopColor()
		{
			try
			{
				Console.ForegroundColor = m_ConsoleColors.Pop();
			}
			catch
			{
			}
		}

		public static string OutputLanguage( Language lang )
		{
			switch ( lang )
			{
				default: return "Invalid"; break;
				case Language.Japanese: return "日本語"; break;
				case Language.English: return "English"; break;
				case Language.French: return "Français"; break;
				case Language.Italian: return "Italiano"; break;
				case Language.German: return "Deutsch"; break;
				case Language.Spanish: return "Español"; break;
				case Language.Korean: return "한국어"; break;
			}
		}

		public static readonly DateTime[] DPDates = new DateTime[]
		{
			new DateTime( 2006, 09, 28 ), // Japan
			new DateTime( 2007, 04, 22 ), // N A
			new DateTime( 2007, 07, 27 ), // Europe
			new DateTime( 2008, 02, 14 ) //Korea
		};

		public static readonly DateTime[] PtDates = new DateTime[]
		{
			new DateTime( 2008, 09, 13 ), //Japan
			new DateTime( 2009, 03, 22 ), //N A
			new DateTime( 2009, 05, 22 ), //Europe
			new DateTime( 2009, 07, 02 ) //Korea
		};

		public static readonly DateTime[] HGSSDates = new DateTime[]
		{
			new DateTime( 2009, 09, 12 ), //Japan
			new DateTime( 2010, 03, 14 ), //N A
			new DateTime( 2010, 03, 26 ), //Europe
			new DateTime( 2010, 02, 4 ) //Korea
		};

		public static bool IsIVMatch( int iv1o, int iv2o, int iv1f, int iv2f )
		{
			return ( iv1o == iv1f ) && ( iv2o == iv2f );
		}

		public static readonly ushort[] HoneyLocations = new ushort[]
		{
			20, //Route 205 South
			20, //Route 205 North
			21, //Route 206
			22, //Route 207
			23, //Route 208
			24, //Route 209
			25, //Route 210 South
			25, //Route 210 North
			26, //Route 211
			27, //Route 212 East
			27, //Route 212 West
			28, //Route 213
			29, //Route 214
			30, //Route 215
			33, //Route 218
			36, //Route 221
			37, //Route 222
			47, //WindWorks
			48, //Eterna
			49, //Feugo
			58, //Floaroma
		};

		public ushort[] ToArray( byte[] data )
		{
			return ToArray( data, 0, data.Length );
		}

		public ushort[] ToArray( byte[] data, int index, int count )
		{
			if ( index < 0 )
				throw new Exception( "Index may not be negative." );

			if ( count <= 0 )
				throw new Exception( "Count may not be negative or zero." );

			if ( index + count > data.Length )
				throw new Exception( "Count may not exceed length of the array." );

			if ( data % 2 != 0 ) //Not an even amount of bytes.
				return new ushort[0];

			ushort[] udata = new ushort[count / 2];

			for ( int i = 0; i < count; i += 2 )
				udata[i/2] = BitConverter.ToUInt16( data, index + i );

			return udata;
		}

		public static bool Append( ref string first, string second, bool prependcomma )
		{
			first = String.Format( "{0}{1}{2}", first, prependcomma ? ", " : "", second );
			return true;
		}

		public static byte GetLevel( int natid, uint exp )
		{
			GrowthType growth = GrowthTypes[natid];

			int[] growthtable = null;

			switch ( growth )
			{
				default: case GrowthType.Slow: growthtable = m_SlowExpByLevel; break;
				case GrowthType.Fast: growthtable = m_FastExpByLevel; break;
				case GrowthType.Medium: growthtable = m_MediumExpByLevel; break;
				case GrowthType.Parabolic: growthtable = m_ParabolicExpByLevel; break;
				case GrowthType.Flux: growthtable = m_FluxExpByLevel; break;
				case GrowthType.Erratic: growthtable = m_ErraticExpByLevel; break;
			}

			if ( exp < growthtable[2] )
				return 1;
			else if ( exp >= growthtable[growthtable.Length-1] )
				return 100;

			for ( byte i = 3; i < growthtable.Length; i++ )
				if ( exp >= growthtable[i] && exp < growthtable[i+1] )
					return i;

			return 0;
		}

		private static int[] m_SlowExpByLevel = new int[]
			{
				0,
				1, 10, 33, 80, 156, 270, 428, 640, 911, 1250,
				1663, 2160, 2746, 3430, 4218, 5120, 6141, 7290, 8573, 10000,
				11576, 13310, 15208, 17280, 19531, 21970, 24603, 27440, 30486, 33750,
				37238, 40960, 44921, 49130, 53593, 58320, 63316, 68590, 74148, 80000,
				86151, 92610, 99383, 106480, 113906, 121670, 129778, 138240, 147061, 156250,
				165813, 175760, 186096, 196830, 207968, 219520, 231491, 243890, 256723, 270000,
				283726, 297910, 312558, 327680, 343281, 359370, 375953, 393040, 410636, 428750,
				447388, 466560, 486271, 506530, 527343, 548720, 570666, 593190, 616298, 640000,
				664301, 689210, 714733, 740880, 767656, 795070, 823128, 851840, 881211, 911250,
				941963, 973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
			};

		private static int[] m_FastExpByLevel = new int[]
			{
				0,
				0, 6, 21, 51, 100, 172, 274, 409, 583, 800,
				1064, 1382, 1757, 2195, 2700, 3276, 3930, 4665, 5487, 6400,
				7408, 8518, 9733, 11059, 12500, 14060, 15746, 17561, 19511, 21600,
				23832, 26214, 28749, 31443, 34300, 37324, 40522, 43897, 47455, 51200,
				55136, 59270, 63605, 68147, 72900, 77868, 83058, 88473, 94119, 100000,
				106120, 112486, 119101, 125971, 133100, 140492, 148154, 156089, 164303, 172800,
				181584, 190662, 200037, 209715, 219700, 229996, 240610, 251545, 262807, 274400,
				286328, 298598, 311213, 324179, 337500, 351180, 365226, 379641, 394431, 409600,
				425152, 441094, 457429, 474163, 491300, 508844, 526802, 545177, 563975, 583200,
				602856, 622950, 643485, 664467, 685900, 707788, 730138, 752953, 776239, 800000,
			};

		private static int[] m_MediumExpByLevel = new int[]
			{
				0,
				1, 8, 27, 64, 125, 216, 343, 512, 729, 1000,
				1331, 1728, 2197, 2744, 3375, 4096, 4913, 5832, 6859, 8000,
				9261, 10648, 12167, 13824, 15625, 17576, 19683, 21952, 24389, 27000,
				29791, 32768, 35937, 39304, 42875, 46656, 50653, 54872, 59319, 64000,
				68921, 74088, 79507, 85184, 91125, 97336, 103823, 110592, 117649, 125000,
				132651, 140608, 148877, 157464, 166375, 175616, 185193, 195112, 205379, 216000,
				226981, 238328, 250047, 262144, 274625, 287496, 300763, 314432, 328509, 343000,
				357911, 373248, 389017, 405224, 421875, 438976, 456533, 474552, 493039, 512000,
				531441, 551368, 571787, 592704, 614125, 636056, 658503, 681472, 704969, 729000,
				753571, 778688, 804357, 830584, 857375, 884736, 912673, 941192, 970299, 1000000,
			};

		private static int[] m_ParabolicExpByLevel = new int[]
			{
				0,
				0, 9, 57, 96, 135, 179, 236, 314, 419, 560,
				742, 973, 1261, 1612, 2035, 2535, 3120, 3798, 4575, 5460,
				6458, 7577, 8825, 10208, 11735, 13411, 15244, 17242, 19411, 21760,
				24294, 27021, 29949, 33084, 36435, 40007, 43808, 47846, 52127, 56660,
				61450, 66505, 71833, 77440, 83335, 89523, 96012, 102810, 109923, 117360,
				125126, 133229, 141677, 150476, 159635, 169159, 179056, 189334, 199999, 211060,
				222522, 234393, 246681, 259392, 272535, 286115, 300140, 314618, 329555, 344960,
				360838, 377197, 394045, 411388, 429235, 447591, 466464, 485862, 505791, 526260,
				547274, 568841, 590969, 613664, 636935, 660787, 685228, 710266, 735907, 762160,
				789030, 816525, 844653, 873420, 902835, 932903, 963632, 995030, 1027103, 1059860,
			};

		private static int[] m_FluxExpByLevel = new int[]
			{
				0,
				0, 4, 13, 32, 65, 112, 178, 276, 393, 540,
				745, 967, 1230, 1591, 1957, 2457, 3046, 3732, 4526, 5440,
				6482, 7666, 9003, 10506, 12187, 14060, 16140, 18439, 20974, 23760,
				26811, 30146, 33780, 37731, 42017, 46656, 50653, 55969, 60505, 66560,
				71677, 78533, 84277, 91998, 98415, 107069, 114205, 123863, 131766, 142500,
				151222, 163105, 172697, 185807, 196322, 210739, 222231, 238036, 250562, 267840,
				281456, 300293, 315059, 335544, 351520, 373744, 390991, 415050, 433631, 459620,
				479600, 507617, 529063, 559209, 582187, 614566, 639146, 673863, 700115, 737280,
				765275, 804997, 834809, 877201, 908905, 954084, 987754, 1035837, 1071552, 1122660,
				1160499, 1214753, 1254796, 1312322, 1354652, 1415577, 1460276, 1524731, 1571884, 1640000
			};

		private static int[] m_ErraticExpByLevel = new int[]
			{
				0,
				2, 23, 79, 186, 362, 622, 980, 1454, 2055, 2800,
				3700, 4769, 6019, 7463, 9112, 10977, 13068, 15396, 17970, 20800,
				23893, 27258, 30904, 34836, 39062, 43588, 48420, 53562, 59021, 64800,
				70902, 77332, 84092, 91185, 98612, 106375, 114475, 122913, 131688, 140800,
				150247, 160030, 170144, 180590, 191362, 202458, 213875, 225607, 237650, 125000,
				131324, 137795, 144410, 151165, 158056, 165079, 172229, 179503, 186894, 194400,
				202013, 209728, 217540, 225443, 233431, 241496, 249633, 257834, 267406, 276458,
				286328, 296358, 305767, 316074, 326531, 336255, 346965, 357812, 367807, 378880,
				390077, 400293, 411686, 423190, 433572, 445239, 457001, 467489, 479378, 491346,
				501878, 513934, 526049, 536557, 548719, 560922, 571333, 583539, 591882, 600000
			};

		public static readonly ushort[] EvoChains = new ushort[]
			{
				0,
				1,1,2,4,4,5,7,7,8,10,
				10,11,13,13,14,16,16,17,19,19,
				21,21,23,23,172,25,27,27,29,29,
				30,32,32,33,173,35,37,37,174,39,
				41,41,43,43,44,46,46,48,48,50,
				50,52,52,54,54,56,56,58,58,60,
				60,61,63,63,64,66,66,67,69,69,
				70,72,72,74,74,75,77,77,79,79,
				81,81,83,84,84,86,86,88,88,90,
				90,92,92,93,95,96,96,98,98,100,
				100,102,102,104,104,236,236,108,109,109,
				111,111,440,114,115,116,116,118,118,120,
				120,439,123,238,239,240,127,128,129,129,
				131,132,133,133,133,133,137,138,138,140,
				140,142,446,144,145,146,147,147,148,150,
				151,152,152,153,155,155,156,158,158,159,
				161,161,163,163,165,165,167,167,42,170,
				170,172,173,174,175,175,177,177,179,179,
				180,44,298,183,438,61,187,187,188,190,
				191,191,193,194,194,133,133,198,79,200,
				201,360,203,204,204,206,207,95,209,209,
				211,123,213,214,215,216,216,218,218,220,
				220,222,223,223,225,458,227,228,228,117,
				231,231,137,234,235,236,236,238,239,240,
				241,113,243,244,245,246,246,247,249,250,
				251,252,252,253,255,255,256,258,258,259,
				261,261,263,263,265,265,266,265,268,270,
				270,271,273,273,274,276,276,278,278,280,
				280,281,283,283,285,285,287,287,288,290,
				290,290,293,293,294,296,296,298,299,300,
				300,302,303,304,304,305,307,307,309,309,
				311,312,313,314,406,316,316,318,318,320,
				320,322,322,324,325,325,327,328,328,329,
				331,331,333,333,335,336,337,338,339,339,
				341,341,343,343,345,345,347,347,349,349,
				351,352,353,353,355,355,357,433,359,360,
				361,361,363,363,364,366,366,366,369,370,
				371,371,372,374,374,375,377,378,379,380,
				381,382,383,384,385,386,387,387,388,390,
				390,391,393,393,394,396,396,397,399,399,
				401,401,403,403,404,406,315,408,408,410,
				410,412,412,412,415,415,417,418,418,420,
				420,422,422,190,425,425,427,427,200,198,
				431,431,433,434,434,436,436,438,439,440,
				441,442,443,443,444,446,447,447,449,449,
				451,451,453,453,455,456,456,458,459,459,
				215,82,108,112,114,125,126,176,193,133,
				133,207,221,233,281,299,356,361,479,480,
				481,482,483,484,485,486,487,488,489,489,
				491,492,493
			};

		public static readonly byte[] GenderThresholds = new byte[]
			{
				31,31,31,31,31,31,31,31,31,127,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,254,254,
				254,0,0,0,191,191,191,191,191,191,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,63,63,127,
				127,127,63,63,63,63,63,63,127,127,
				127,127,127,127,127,127,127,127,127,127,
				255,255,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,127,255,
				255,127,127,127,127,0,0,127,127,127,
				127,127,254,127,254,127,127,127,127,255,
				255,127,127,254,63,63,127,0,127,127,
				127,255,31,31,31,31,255,31,31,31,
				31,31,31,255,255,255,127,127,127,255,
				255,31,31,31,31,31,31,31,31,31,
				127,127,127,127,127,127,127,127,127,127,
				127,127,191,191,31,31,127,127,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,31,31,127,127,127,
				255,127,127,127,127,127,127,127,191,191,
				127,127,127,127,127,127,127,127,127,127,
				127,191,127,127,127,127,127,127,127,127,
				127,127,255,127,127,0,0,254,63,63,
				254,254,255,255,255,127,127,127,255,255,
				255,31,31,31,31,31,31,31,31,31,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,255,127,127,127,63,63,191,127,191,
				191,127,127,127,127,127,127,127,127,127,
				127,127,0,254,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,255,255,127,127,
				127,127,255,255,31,31,31,31,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,127,127,127,127,127,127,127,31,191,
				127,127,127,255,255,255,255,255,255,254,
				0,255,255,255,255,255,31,31,31,31,
				31,31,31,31,31,127,127,127,127,127,
				127,127,127,127,127,127,127,31,31,31,
				31,127,254,0,31,254,127,127,127,127,
				127,127,127,127,127,127,127,127,127,127,
				191,191,127,127,127,255,255,127,127,254,
				127,127,127,127,127,31,31,31,127,127,
				127,127,127,127,127,127,127,127,127,127,
				127,255,127,127,127,63,63,31,127,31,
				31,127,127,255,0,127,127,254,255,255,
				255,255,255,255,127,255,255,254,255,255,
				255,255,255
			};

		public static readonly GrowthType[] GrowthTypes = new GrowthType[]
			{
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Fast,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Fast,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Cubic, GrowthType.Fast,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Cubic, GrowthType.Fast, GrowthType.Fast,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Parabolic, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Parabolic, GrowthType.Fast, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Slow, GrowthType.Slow, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Erratic,
				GrowthType.Erratic, GrowthType.Erratic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Slow, GrowthType.Slow, GrowthType.Fast, GrowthType.Cubic, GrowthType.Fast,
				GrowthType.Fast, GrowthType.Flux, GrowthType.Fast, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Erratic, GrowthType.Slow, GrowthType.Flux, GrowthType.Slow, GrowthType.Slow, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Slow,
				GrowthType.Slow, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Slow, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Slow, GrowthType.Slow, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic,
				GrowthType.Cubic, GrowthType.Flux, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Parabolic, GrowthType.Fast, GrowthType.Flux, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Parabolic, GrowthType.Fast,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Flux, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Erratic,
				GrowthType.Erratic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Flux, GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Slow, GrowthType.Slow, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Flux,
				GrowthType.Fast, GrowthType.Fast, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast,
				GrowthType.Flux, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Flux, GrowthType.Flux, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Erratic, GrowthType.Erratic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Flux, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic,
				GrowthType.Cubic, GrowthType.Flux, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Parabolic, GrowthType.Cubic, GrowthType.Fast, GrowthType.Cubic, GrowthType.Cubic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic, GrowthType.Parabolic,
				GrowthType.Parabolic, GrowthType.Flux, GrowthType.Parabolic
			};
	}
}