----------------------------------------------------
-- Project: MusiC.Extensions.Classifiers.uBarbedo --
----------------------------------------------------

project("MusiC.Extensions.Classifiers.uBarbedo")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c++")

files({
	base_src_dir.."/Extensions/Classifiers/Barbedo/**.cpp",
	base_src_dir.."/Extensions/Classifiers/Barbedo/**.h"
})

includedirs({
	base_src_dir.."/Native",
	base_deps_dir.."/include"
})

links({
	"gsl",
	"gslcblas",
	"MusiC.Native.Base"
})

libdirs({
	base_deps_dir.."/"..os.get().."/lib",
	base_bin_dir
})

---------------------
-- Code Generation --
---------------------

flags({"Unicode", "NoImportLib"})

-- linkoptions({})

------------
-- Output --
------------

kind("SharedLib")

targetname("MusiC.Extensions.Classifiers.uBarbedo")
targetprefix("")
targetextension(".dll")
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
