# State-machine portfolio and fixes context

Status: initial comparison complete; awaiting scope clarification before edits.

## User-provided history
- Solo project, built because explored state-machine/behavior-tree solutions did not fit desired player transitions.
- User later recognized similarities to hierarchical state machines.
- Intended to reuse ready states and tweakable configuration in new projects.
- User reports fixes were made in Parrying_Game and requests porting them to the freshly cloned public FSM repository.

## Local repositories
- Source: C:/Users/kuruttakao/Documents/GitHub/Parrying_Game
- Target: C:/Users/kuruttakao/Documents/GitHub/SOLID_Unity_Personalized_FSM
- Target was clean at initial inspection.
- Source AGENTS.md identifies Unity 6000.3.5f2; target ProjectVersion.txt identifies 2022.3.62f2.

## Initial source evidence
- Assets/2D/_Shared/StateMachineCore/StateMachineCore.cs defines generic machine, factory, and state bases.
- Player StateFactoryPlayer.CreateState uses ScriptableObject.Instantiate(stateAsset), isolating runtime state per machine.
- PlayerState derives from generic StateBase; StateMachinePlayer derives from generic StateMachineBase.
- StateBase includes IsCurrentState; usages and transition ordering still need full review.
- Source has player and mob specializations; migrating its architecture is broader than patching the original target API.

## Pending decision
Conservative backport retaining target APIs and Unity 2022 compatibility, or migrate to generic player/enemy architecture as well?

## Validation and attribution
No code has been changed or Unity compilation/runtime testing performed in this task yet.
Keep user-authored source improvements distinct from additional fixes introduced during this port.
Do not claim fully general HSM semantics, universal reusability, or measured outcomes without evidence.

## Migration progress — 2026-10-05
- User chose generic-core migration, leaving Unity version unchanged.
- Ported generic machine/state/factory and configuration interfaces from Parrying_Game.
- Kept original player adapter class names and script GUIDs; retained separate Context to preserve existing serialized references.
- Ported per-machine cloning and active-state guards; existing condition order remains transition priority.
- Added stale/self transition rejection, active-state assignment before OnEnter, timer resets, runtime clone disposal, missing-state diagnostics, and corrected RootState trigger-stay forwarding.
- Removed animation coupling from generic core; sample State adapter owns animation.
- Updated target README to match implemented API, configuration menus, reuse boundaries, and validation workflow.
- Added Editor regression validation for clone isolation, lifecycle ordering, stale/self requests, missing-state reporting, and cleanup.
- Initial Unity 2022.3 compilation succeeded; fresh-project asset import and final validation still underway.
- Parrying_Game remains read-only. No portfolio page has been added yet.

## Final validation
- Unity 2022.3.62f2 batch import/compilation completed with exit code 0.
- FsmPortValidation.Run completed with FSM_PORT_VALIDATION_PASSED and exit code 0.
- Verified isolated runtime clones, unchanged template assets, lifecycle ordering, stale/self-transition rejection, missing-state diagnostics, and clone cleanup.
- No semantic ProjectSettings or Packages diff; no scene/prefab changes authored.
- Initial compile reported an existing GroundNpcState.Destroy hiding warning. Headless shader import warnings do not establish rendering correctness.
- Interactive sample-scene movement/combat and visual validation have NOT been performed.

## Suggested portfolio evidence
Show the original transition problem, the progression into a generic core, player versus enemy use in Parrying_Game, Inspector-configured states, and the multi-instance regression check. Attribute the additional hardening in this port separately from the original solo work. Keep the project described as inheritance-based shared state behavior unless explicit hierarchical lifecycle semantics are implemented later.
