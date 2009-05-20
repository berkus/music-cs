--------------------
-- Project: MusiC --
--------------------

project("MusiC.Test.Unit")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/MusiC/Test/Unit/**.cs"
})

includedirs({
})

links({
	"System",
	"System.Core",
	"MusiC",
	"log4net"
})

libdirs({
	base_bin_dir,
	base_deps_dir.."/DotNet"
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

targetname("MusiC.Test.Unit")
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")

