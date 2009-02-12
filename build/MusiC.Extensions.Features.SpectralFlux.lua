-----------------------------------------------------
-- Project: MusiC.Extensions.Features.SpectralFlux --
-----------------------------------------------------

project("MusiC.Extensions.Features.SpectralFlux")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/Extensions/Features/SpectralFlux/**.cs"
})

includedirs({
})

links({
	"MusiC"
})

libdirs({
	base_bin_dir
})

---------------------
-- Code Generation --
---------------------

defines({"TRACE"})

configuration({"Debug"})
	defines({"DEBUG"})
configuration({})

flags({"Unsafe"})

-- linkoptions({})

------------
-- Output --
------------

kind("SharedLib")

targetname("MusiC.Extensions.Features.SpectralFlux")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
