These are the prefabs for the DMX via Stream Panel variants of VRSL and a short description of each.

Fixtures that have the word "Static" will be the most performant version of that fixture. This is because there are no update loops running on the script. The difference between the variants that have "Static" and the ones that don't will be explained below.

VRSL-DMX-Mover-Spotlight - The VRSL Spot Light with DMX support. This is the "Mover" variant. This means that it will have the ability to follow a "target" by having a target transform variable and enabling "Follow Target" and disabling DMX. It is not recommended to use this version unless you intend for this fixture to stop reading DMX and follow a target during runtime.

VRSL-DMX-Static-Spotlight - The VRSL Spot Light with DMX support. This is the "Static" variant. This means it will not have the ability to follow a "target". It is recommeneded to use this version of the spotlight as it will have all the same DMX movement functionality as the "Mover" variant, but be signifigantly more performant.

VRSL-DMX-Static-Blinder - The VRSL Blinder with DMX support.

VRSL-DMX-Static-LightBar - The VRSL Light Bar with DMX support.

VRSL-DMX-Static-ParLight - The VRSL Par Light with DMX support.

VRSL-DMX-Static-DiscoBall - The VRSL Disco ball with DMX support. Defaults to sector 39.




