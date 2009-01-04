-- MusiC.Extensions.Classifiers.uBarbedo

package = newpackage()
package.name="MusiC.Native"
package.path=project.path

-- Input
package.language="c++"
package.files={
	matchrecursive(base_src_dir.."/Native/*.cpp"),
	matchrecursive(base_src_dir.."/Native/*.h")
}
package.links={
}
package.libpaths={
}
package.includepaths={
}

-- Code Generation
--package.defines={"TRACE"}
package.buildflags={"dylib"}

-- Output
package.kind="dll"
package.target="MusiC.Native"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir
package.objdir=base_bin_dir.."/obj"

if (windows) then
	package.buildoptions={
		"/IMPLIB:"..base_bin_dir.."/MusiC.Native.lib"	
	}
end

if (linux or macosx) then
	package.linkoptions={
		"-Wl,--out-implib,"..base_bin_dir.."/libMusic.Native.a"
	}
end

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
