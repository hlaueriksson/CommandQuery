# CommandQuery.Sample

Invoke the `FooCommandHandler` with:

`curl -X POST -d "{'Value':'Foo'}" http://localhost:57857/api/command/FooCommand --header "Content-Type:application/json"`

Invoke the `BarQueryHandler` with:

`curl -X POST -d "{'Id':1}" http://localhost:57857/api/query/BarQuery --header "Content-Type:application/json"`
