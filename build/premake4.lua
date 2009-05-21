function MakeUnmanagedProjects()
	dofile("MusiC.Native.Base.lua")
	dofile("MusiC.Native.Core.lua")
	dofile("MusiC.Extensions.Classifiers.uBarbedo.lua")
end

function MakeManagedProjects()
	dofile("MusiC.lua")
	dofile("MusiC.Apps.GenreC.lua")
	dofile("MusiC.Extensions.Classifiers.Barbedo.lua")
	dofile("MusiC.Extensions.Configs.XMLConfigurator.lua")
	dofile("MusiC.Extensions.Features.SpecRollOff.lua")
	dofile("MusiC.Extensions.Features.SpectralFlux.lua")
	dofile("MusiC.Extensions.Features.Loudness.lua")
	dofile("MusiC.Extensions.Features.Bandwidth.lua")
	dofile("MusiC.Extensions.Handlers.WAVHandler.lua")
	dofile("MusiC.Extensions.Windows.Hamming.lua")
end

function MakeTestingProjects()
	dofile("MusiC.Test.lua")
end

function CreateProject()
	base_deps_dir = "../deps"
	base_src_dir = "../src"
	base_bin_dir = "../bin"
	base_prj_dir = _ACTION;

	solution("MusiC")
	configurations({"Debug", "Release"})
	location(base_prj_dir)
	
	if 
	_ACTION == "vs2008" or
	_ACTION == "vs2005" 
	then
		MakeManagedProjects()
		MakeTestingProjects()
		
		solution("uMusiC")
		configurations({"Debug", "Release"})
		
		-- this will change the location of the projects.
		base_prj_dir = base_prj_dir.."u"
		location(base_prj_dir)
		
		MakeUnmanagedProjects()
	elseif
	_ACTION == "vs2003" or
	_ACTION == "vs2002" 
	then
		MakeManagedProjects()
	elseif
	_ACTION == "codeblocks" or
	_ACTION == "codelite"
	then
		MakeUnmanagedProjects()
	elseif
	_ACTION == "gmake"
	then
		MakeManagedProjects()
		MakeUnmanagedProjects()
	end
end

function main()
	CreateProject()
end

main()
