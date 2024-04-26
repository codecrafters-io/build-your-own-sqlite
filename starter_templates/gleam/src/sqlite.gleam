import argv
import snag.{type Result}
import gleam/int

pub fn load() -> Result(Options) {
  case argv.load().arguments {
    [] -> Ok(default_options)
    ["--port", port] -> {
      use port <- snag_try(
        int.parse(port),
        "failed to parse port number " <> port,
      )
      Ok(Options(port: port))
    }
    _ -> snag.error("Invalid args")
  }
}
