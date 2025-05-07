#Boris Proxy

Simple proxy with simple config

Create your config:

{
  "run": {
    "sources": [
      <Your Sources>
    ],
    "sinks": [
      <Your Sinks>
    ],
    "routers": [
      <Your Routers>
    ]
  }
}

##Sources
###Struct
{
  "name": <Name>,
  "type": <Type>,
  "conn": <Sink or router conected to>
}
###Types
- HTTPS = Makes a https server
- HTTP = Makes a http server
###HTTPS
Params:
- addr: Address
- port: Port
- cert_path: Private pfx path
- cert_pass: pfx password
###HTTP
- addr: Address
- port: Port

#Sinks
