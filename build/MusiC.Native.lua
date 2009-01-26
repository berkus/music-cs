---------------------------
-- Project: MusiC.Native --
---------------------------

project("MusiC.Native")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c++")

files({
	base_src_dir.."/Native/**.cpp",
	base_src_dir.."/Native/**.h"
})

includedirs({
})

links({
})

libdirs({
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

targetname("MusiC.Native")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")