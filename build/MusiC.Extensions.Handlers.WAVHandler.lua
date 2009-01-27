----------------------------------------------------
-- Project: MusiC.Extensions.Handlers.WAVHandlers --
----------------------------------------------------

project("MusiC.Extensions.Handlers.WAVHandlers")
location(base_prj_dir)

-----------
-- Input --
-----------

language("c#")

files({
	base_src_dir.."/Extensions/Handlers/WAVHandler/**.cs"
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

targetname("MusiC.Extensions.Handlers.WAVHandlers")
--targetprefix("")
--targetextension="dll"
targetdir(base_bin_dir)
objdir(base_bin_dir.."/obj")
