--------------------
-- Project: MusiC --
--------------------

project("MusiC")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/MusiC/**.cs",
	base_src_dir.."/MusiC/**.xml"
})

includedirs({
})

links({
	"System",
	"System.Core",
	"System.Xml",
	"System.Xml.Linq",
	"log4net"
})

libdirs({
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

targetname("MusiC")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")

-- package.config[matchrecursive(base_src_dir.."/*.xml")].buildaction="Content"
