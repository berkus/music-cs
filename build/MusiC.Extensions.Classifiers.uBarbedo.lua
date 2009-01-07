-- MusiC.Extensions.Classifiers.uBarbedo

package = newpackage()
package.name="MusiC.Extensions.Classifiers.uBarbedo"
package.path=project.path

-- Input
package.language="c++"
package.files={
	matchrecursive(base_src_dir.."/Extensions/Classifiers/Barbedo/*.cpp"),
	matchrecursive(base_src_dir.."/Extensions/Classifiers/Barbedo/*.h")
}
package.links={
	"gsl",
	"gslcblas",
}
package.libpaths={
	base_deps_dir.."/"..OS.."/lib",
	base_bin_dir
}

package.includepaths={
	base_src_dir.."/Native",
	base_deps_dir.."/include"
}

-- Code Generation
--package.defines={"TRACE"}
package.buildflags={"dylib", "no-import-lib"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Classifiers.uBarbedo"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir
package.objdir=base_bin_dir.."/obj"

if(compiler == "gcc") then
	package.linkoptions={
		"../../bin/MusiC.Native.dll"
	}
end

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
