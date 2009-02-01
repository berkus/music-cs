--------------------------------
-- Project: MusiC.Native.Core --
--------------------------------

project("MusiC.Native.Core")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c++")

files({
	base_src_dir.."/Native/Core/**.cpp",
	base_src_dir.."/Native/Core/**.h"
})

includedirs({
	base_src_dir.."/Native"
})

links({
	"MusiC.Native.Base"
})

libdirs({
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

targetname("MusiC.Native.Core")
targetprefix("")
targetextension(".dll")
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
