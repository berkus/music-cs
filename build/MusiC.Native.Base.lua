--------------------------------
-- Project: MusiC.Native.Base --
--------------------------------

project("MusiC.Native.Base")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c++")

files({
	base_src_dir.."/Native/*.cpp",
	base_src_dir.."/Native/*.h"
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

flags({"Unicode"})

-- linkoptions({})

------------
-- Output --
------------

kind("StaticLib")

targetname("MusiC.Native.Base")
--targetprefix("")
--targetextension(".dll")
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
