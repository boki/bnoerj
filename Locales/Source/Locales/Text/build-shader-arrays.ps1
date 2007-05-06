# Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
# All rights reserved.
#
# This software is licensed as described in the file license.txt, which
# you should have received as part of this distribution. The terms
# are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

function Get-ByteArray ($path = $(throw "path must be specified"), $shaderName, $width = 16)
{
	"		internal static byte[] $shaderName = new byte[] {$nl"

	Get-Content -Encoding byte $path -ReadCount $width | %{
		$record = $_
		$hex = $record | %{"0x{0:x2}," -f $_}

		"			$hex$nl"
	}

	"		};$nl"
}

[System.Management.Automation.ApplicationInfo]$fxc = Get-Command 'C:\Programme\Microsoft DirectX SDK (April 2007)\Utilities\Bin\x86\fxc' -CommandType Application

& $($fxc.Definition) /nologo /Tvs_2_0 /Gec /EGlyphVS /VnVertexShader /Foglyph.vso /Op glyph.fx
& $($fxc.Definition) /nologo /Tps_2_0 /Gec /EGlyphPS /VnPixelShader  /Foglyph.pso /Op glyph.fx

$nl = [Environment]::NewLine

%{"using System;$nl" +
"$nl" +
"namespace Bnoerj.Locales.Text$nl" +
"{$nl" +
"	internal static class GlyphShaderCode$nl" +
"	{$nl" +

(Get-ByteArray -Path glyph.vso -ShaderName "VertexShader") +
(Get-ByteArray -Path glyph.pso -ShaderName "PixelShader") +

"	}$nl" +
"}" |
Set-Content -Encoding ascii GlyphShaderCode.cs }
