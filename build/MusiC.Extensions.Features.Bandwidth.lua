--------------------------------------------------
-- Project: MusiC.Extensions.Features.Bandwidth --
--------------------------------------------------

project("MusiC.Extensions.Features.Bandwidth")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/Extensions/Features/Bandwidth/**.cs"
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

targetname("MusiC.Extensions.Features.Bandwidth")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")