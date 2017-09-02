# Windows

Build
* `msbuild` or Visual Studio 2015+

Run
* `.\Tydbits.LruCache\bin\Debug\LruCache.exe` or
* `.\Tydbits.LruCache\bin\Release\LruCache.exe`

# Mono

Prerequisites
* mono
* nuget

Build (from the .sln dir)
* `nuget restore`
* `xbuild`

Run
* `mono ./Tydbits.LruCache/bin/Debug/LruCache.exe` or
* `mono ./Tydbits.LruCache/bin/Release/LruCache.exe`

# Testing

IDE
* Use your favorite NUnit runner

Mono command line
* install `NUnit`
* `xbuild`
* `nunit-console ./Tydbits.LruCache.Tests/bin/Debug/Tydbits.LruCache.Tests.dll` or
* `nunit-console ./Tydbits.LruCache.Tests/bin/Release/Tydbits.LruCache.Tests.dll`

# Implementation

The `LruCache` implements an LRU (Least Recently Used) cache via a `LinkedList` of `Entry {Key, Value}` indexed by a `Dictionary` (hash map) of `LinkedListNode`.
* Keys and values are treated as strings.
* Neither keys, nor values can contain spaces.

## Commands

`LruCache.exe` reads the following commands from `stdin` (one per line, case insensitive, extra whitespace is insignificant), and prints output to `stdout` (one line per the corresponding command):

* `SIZE {n}` - sets the cache size to `n`, must only occur as the first operation, outputs:
    - -> `SIZE OK` if this is the first operation, and `n` is an integer > 0.
    - -> `ERROR` otherwise
    - Complexity:
    - `O(1)`:
        - `O(1)` create empty hash map
        - `O(1)` create empty linked list
* `GET {key}` - gets an item from the cache, marks the item as most recently used, outputs:
    - -> `GOT {value}` if the item is found successfully
    - -> `NOTFOUND` if the item isn't in the cache
    - -> `ERROR` if syntax isn't valid
    - Complexity:
    - `O(1)` (amortized):
        - `O(1)` (amortized) look up a `LinkedListNode` in the hash map
        - if found
            - `O(1)` remove the `LinkedListNode` from the list
            - `O(1)` add the `LinkedListNode` to the end of the linked list
* `SET {key} {value}` - stores the `value` in the cache under the `key`, marks the item as most recently used, outputs:
    - -> `SET OK` if the item is stored successfully
    - -> `ERROR` if syntax isn't valid
    - Complexity:
    - `O(1)` (amortized):
        - `O(1)` (amortized) look up a `LinkedListNode` in the hash map
        - if found
            - `O(1)` remove the `LinkedListNode` from the list
            - `O(1)` add the `LinkedListNode` to the end of the linked list
            - `O(1)` update the `Entry {Value}`
        - otherwise:
            - `O(1)` add new `Entry {Key, Value}` to the end of the list
            - `O(1)` add the `LinkedListNode` to the hash map
* `EXIT`
    -> terminates the execution
    - Complexity:
    - `O(1)`
