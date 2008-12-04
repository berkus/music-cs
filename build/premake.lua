function MakeUnmanagedProjects()
	dofile("MusiC.Extensions.Classifiers.uBarbedo.lua")
	dofile("MusiC.Extensions.Handlers.uDBHandler.lua")
end

function MakeManagedProjects()
	dofile("MusiC.lua")
	dofile("MusiC.Extensions.Classifiers.Barbedo.lua")
	dofile("MusiC.Extensions.Configs.XMLConfig.lua")
	dofile("MusiC.Extensions.Features.SpecRollOff.lua")
	dofile("MusiC.Extensions.Handlers.DBHandler.lua")
	dofile("MusiC.Extensions.Handlers.WAVHandler.lua")
	dofile("MusiC.Extensions.Windows.Hamming.lua")
end

project.name="MusiC"
project.bindir="../bin"
project.configs={"Debug", "Release"}

if
target == "vs2002" or
target == "vs2003" or
target == "vs2005" or
target == "vs2008"
then
	--Visual Studio supports both managed and unmanaged, but separately
	if options["unmanaged"] or options["umng"] then
		MakeUnmanagedProjects()
	else
		MakeManagedProjects()
	end
elseif
target=="sharpdev" or
target=="monodev"
then
    -- Those support only managed
    MakeManagedProjects()
elseif
target=="cb-gcc" or
target=="cb-ow" or
target=="cl-gcc" or
target=="vs6"
then
	-- Those support only unmanaged code
	MakeUnmanagedProjects()
elseif
target=="gnu"
then
	-- Build both
	MakeManagedProjects()
	MakeUnmanagedProjects()
end
