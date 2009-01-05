function MakeUnmanagedProjects()
	dofile("MusiC.Native.lua")
	dofile("MusiC.Extensions.Classifiers.uBarbedo.lua")
	dofile("MusiC.Extensions.Handlers.uDBHandler.lua")
end

function MakeManagedProjects()
	dofile("MusiC.lua")
	dofile("MusiC.Apps.GenreC.lua")
	dofile("MusiC.Extensions.Classifiers.Barbedo.lua")
	dofile("MusiC.Extensions.Configs.XMLConfigurator.lua")
	dofile("MusiC.Extensions.Features.SpecRollOff.lua")
	dofile("MusiC.Extensions.Handlers.DBHandler.lua")
	dofile("MusiC.Extensions.Handlers.WAVHandler.lua")
	dofile("MusiC.Extensions.Windows.Hamming.lua")
end

function CreateProject()
	addoption("unmanaged", "Switch to build CPP code when it is available.")

	base_deps_dir="../../deps"
	base_src_dir="../../src"
	base_bin_dir="../../bin"
	base_prj_dir="./"..target;


	project.name="MusiC"
	project.bindir=base_bin_dir
	project.configs={"Debug", "Release"}
	project.path="./"..target
	
	if
	target=="vs2003" or
	target=="vs2005" or
	target=="vs2008"
	then
		--Visual Studio supports both managed and unmanaged, but separately
		
		project.path=base_prj_dir.."-unmanaged"
		MakeUnmanagedProjects()
		
		project.path=base_prj_dir.."-managed"
		MakeManagedProjects()
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
	target=="vs2002" or
	target=="vs6"
	then
		-- Those support only unmanaged code
		-- vs2002 dont support .net 2.0
		MakeUnmanagedProjects()
	elseif
	target=="gnu"
	then
		-- Build both
		MakeManagedProjects()
		MakeUnmanagedProjects()
	end
end

function main()
	if target~=nil then CreateProject() end
end

main()
