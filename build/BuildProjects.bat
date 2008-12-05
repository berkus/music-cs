@ECHO off

:menu
cls
echo.
echo.
echo.
echo                             MusiC
echo.
echo.
echo.
echo  Choose one of the alternatives(NOTE they are case sensitive):
echo.
echo  v: Visual Studio         g: GNU Makefile       C: CodeLite
echo  c: Code::Blocks          m: Monodevelop        s: Sharpdevelop
echo.
echo  x: Exit
echo.

set /p compiler=      Enter a letter:

if %compiler%==v goto visualstudio
if %compiler%==c goto codeblocks
if %compiler%==g goto build
if %compiler%==m goto build
if %compiler%==s goto build
if %compiler%==C goto build
if %compiler%==x goto exit
goto menu

:visualstudio
cls
echo.
echo Choose VisualStudio version:
echo.
echo 0: Go Back               2: Visual Studio 2002**        4: Visual Studio 2005
echo 1: Visual Studio 6*      3: Visual Studio 2003          5: Visual Studio 2008
echo.
echo   *(don't support managed code)     **(don't support .Net 2.0)
echo.
set /p version=      Enter a number:

if %version%==0 goto menu
if %version%==1 goto build
if %version%==2 goto build
if %version%==3 goto vs-project-selection
if %version%==4 goto vs-project-selection
if %version%==5 goto vs-project-selection
goto visualstudio

:vs-project-selection
echo.
echo.
echo  Which projects should i create ?
echo.
echo  m: Managed        u: Unmanaged        b: Both
echo.
set /p vsprojects=     Enter a letter:

if %vsprojects%==m goto build
if %vsprojects%==u goto build
if %vsprojects%==b goto build

goto vs-project-selection

:codeblocks
cls
echo.
echo Which compiler you would like to use ?
echo.
echo n: Go Back             o: Open Watcom
echo g: MinGW (GCC)
echo.
set /p version=    Enter an option:

if %version%==n goto menu
if %version%==o goto build
if %version%==g goto build
goto codeblocks

:build
cls
echo.
REM echo [Options] Compiler:%compiler%, Version:%version%, Projects:%vsprojects%
echo.

if %compiler%==g (
	premake --dotnet mono2 --target gnu
	goto pause-before-menu
)

if %compiler%==v (
	if %version%==1 (
		premake --target vs6
		goto pause-before-menu
	)
	if %version%==2 (
		premake --target vs2002
		goto pause-before-menu
	)
	if %version%==3 (
		if %vsprojects%==m premake --target vs2003
		if %vsprojects%==u premake --unmanaged --target vs2003
		if %vsprojects%==b premake --target vs2003
		if %vsprojects%==b premake --unmanaged --target vs2003
		goto pause-before-menu
	)
	if %version%==4 (
		if %vsprojects%==m premake --target vs2005
		if %vsprojects%==u premake --unmanaged --target vs2005
		if %vsprojects%==b premake --target vs2005
		if %vsprojects%==b premake --unmanaged --target vs2005
		goto pause-before-menu
	)
	if %version%==5 (
		if %vsprojects%==m premake --target vs2008
		if %vsprojects%==u premake --unmanaged --target vs2008
		if %vsprojects%==b premake --target vs2008
		if %vsprojects%==b premake --unmanaged --target vs2008
		goto pause-before-menu
	)
)

if %compiler%==c (
	if %version%==o (
		premake --target cb-ow
		goto pause-before-menu
	)
	if %version%==g (
		premake --target cb-gcc
		goto pause-before-menu
	)
)

if %compiler%==C (
	premake --target cl-gcc
	goto pause-before-menu
)

if %compiler%==m (
	premake --dotnet mono2 --target monodev
	goto pause-before-menu
)

if %compiler%==s (
	premake --dotnet mono2 --target sharpdev
	goto pause-before-menu
)

:pause-before-menu
echo.
echo.
pause
goto menu

:exit
