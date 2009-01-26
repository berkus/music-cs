--------------------------------
-- Project: MusiC.Apps.GenreC --
--------------------------------

project("MusiC.Apps.GenreC")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/Apps/GenreC/**.cs",
	base_src_dir.."/Apps/GenreC/config.xml"
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

kind("ConsoleApp")

targetname("MusiC.Apps.GenreC")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
