﻿using System;
using NUnit.Framework;
using System.IO;

namespace SkiaSharp.Tests
{
	public class SKTypefaceTest : SKTest
	{
		static readonly string[] ExpectedTablesSpiderFont = new string[] {
			"FFTM", "GDEF", "OS/2", "cmap", "cvt ", "gasp", "glyf", "head",
			"hhea", "hmtx", "loca", "maxp", "name", "post",
		};

		static readonly int[] ExpectedTableLengthsReallyBigAFont = new int[] {
			28, 30, 96, 330, 4, 8, 236, 54, 36, 20, 12, 32, 13665, 44,
		};

		static readonly byte[] ExpectedTableDataPostReallyBigAFont = new byte[] {
			0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xF1, 0x00, 0x06, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00,
			0x02, 0x00, 0x24, 0x00, 0x44,
		};

		[Test]
		public void NullInWrongFileName()
		{
			Assert.Null(SKTypeface.FromFile(Path.Combine(PathToFonts, "font that doesn't exist.ttf")));
		}

		[Test]
		public void TestFamilyName()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "Roboto2-Regular_NoEmbed.ttf")))
			{
				if (IsLinux) // see issue #225
					Assert.AreEqual("", typeface.FamilyName);
				else
					Assert.AreEqual("Roboto2", typeface.FamilyName);
			}
		}

		private static string GetReadableTag(uint v)
		{
			return
				((char)((v >> 24) & 0xFF)).ToString() +
				((char)((v >> 16) & 0xFF)).ToString() +
				((char)((v >> 08) & 0xFF)).ToString() +
				((char)((v >> 00) & 0xFF)).ToString();
		}

		private static uint GetIntTag(string v)
		{
			return
				(UInt32)(v[0]) << 24 |
				(UInt32)(v[1]) << 16 |
				(UInt32)(v[2]) << 08 |
				(UInt32)(v[3]) << 00;
		}

		[Test]
		public void TestGetTableTags()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "SpiderSymbol.ttf")))
			{
				if (IsLinux) // see issue #225
					Assert.AreEqual("", typeface.FamilyName);
				else
					Assert.AreEqual("SpiderSymbol", typeface.FamilyName);
				var tables = typeface.GetTableTags();
				Assert.AreEqual(ExpectedTablesSpiderFont.Length, tables.Length);

				for (int i = 0; i < tables.Length; i++) {
					Assert.AreEqual(ExpectedTablesSpiderFont[i], GetReadableTag(tables[i]));
				}
			}
		}

		[Test]
		public void TestGetTableData()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "ReallyBigA.ttf")))
			{
				if (IsLinux) // see issue #225
					Assert.AreEqual("", typeface.FamilyName);
				else
					Assert.AreEqual("ReallyBigA", typeface.FamilyName);
				var tables = typeface.GetTableTags();

				for (int i = 0; i < tables.Length; i++) {
					byte[] tableData = typeface.GetTableData(tables[i]);
					Assert.AreEqual(ExpectedTableLengthsReallyBigAFont[i], tableData.Length);
				}

				Assert.AreEqual(ExpectedTableDataPostReallyBigAFont, typeface.GetTableData(GetIntTag("post")));

			}
		}

		[Test]
		public void TestFontManagerMatchCharacter()
		{
			var fonts = SKFontManager.Default;
			var emoji = "🚀";
			var emojiChar = StringUtilities.GetUnicodeCharacterCode(emoji, SKTextEncoding.Utf32);
			using (var typeface = fonts.MatchCharacter(emojiChar))
			{
				if (IsLinux)
					Assert.AreEqual("Symbola", typeface.FamilyName);
				else if (IsMac)
					Assert.AreEqual("Apple Color Emoji", typeface.FamilyName);
				else if (IsWindows)
					Assert.Contains(typeface.FamilyName, new[] { "Segoe UI Emoji", "Segoe UI Symbol" });
			}
		}

		[Test]
		public void ExceptionInInvalidGetTableData()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "Distortable.ttf")))
			{
				Assert.Throws<System.Exception>(() => typeface.GetTableData(8));
			}
		}

		[Test]
		public void TestTryGetTableData()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "ReallyBigA.ttf")))
			{
				var tables = typeface.GetTableTags();
				for (int i = 0; i < tables.Length; i++) {
					byte[] tableData;
					Assert.True(typeface.TryGetTableData(tables[i], out tableData));
					Assert.AreEqual(ExpectedTableLengthsReallyBigAFont[i], tableData.Length);
				}

				Assert.AreEqual(ExpectedTableDataPostReallyBigAFont, typeface.GetTableData(GetIntTag("post")));

			}
		}

		[Test]
		public void InvalidTryGetTableData()
		{
			using (var typeface = SKTypeface.FromFile(Path.Combine(PathToFonts, "Distortable.ttf")))
			{
				byte[] tableData;
				Assert.False(typeface.TryGetTableData(8, out tableData));
				Assert.Null(tableData);
			}
		}

	}
}
