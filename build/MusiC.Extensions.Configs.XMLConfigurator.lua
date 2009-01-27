-------------------------------------------------------
-- Project: MusiC.Extensions.Configs.XMLConfigurator --
-------------------------------------------------------

project("MusiC.Extensions.Configs.XMLConfigurator")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/Extensions/Configs/XMLConfigurator/**.cs"
})

includedirs({
})

links({
	"System.Xml",
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

targetname("MusiC.Extensions.Configs.XMLConfigurator")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
