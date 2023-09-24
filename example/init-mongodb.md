## The initialization of MongoDB is a "slightly" manual process.


The intial processor collection need to be created.
Adding of processors is also a manual process and has no interface currently.

1. Create the `apc-processors` collections.

2. Create a processor in that collection, using the following model (subject to change, please see APC.Kernel/Models/Processor.cs).

```json
{
    "_id": "PROCESSOR-NAME", // Name of the processor, this is used for routing.
    "config": {
        "EXTRA-VARIABLE-FIELD": {
            "key": "EXTRA-VARIABLE-FIELD",
            "type": "string",     // Currently has no effect, future will be dropdown etc
            "name": "Group ID",   // Presented to the user
            "placeholder": "DUMMY-GROUP"
        }
    },
    "direct_collect": false,         // Direct collection bypasses routers and does not attempt processing.
    "description": "<b>Test</b>"    // This field is editable in the APC.GUI and supports HTML.
}
```

3. The user interface should now present the `PROCESSOR-NAME` in the processors tab.