# Producing Evidence

[Daniel-Terhorst North defines](https://dannorth.net/2021/07/26/we-need-to-talk-about-testing/),

> *The purpose of testing is to **increase confidence** for **stakeholders** through **evidence**.*

When confronted with that definition, many people ask "*what* is meant by ***evidence*** and *how* do I produce that?"

[There is a paper which offers a possible answer to *what evidence is*](https://ieeexplore.ieee.org/document/9440154). The paper argues that a healthy dose of BDD thinking leads us to boil down the process of testing into identifying values which can then be managed using combinatorial testing, knowledge about business value and common sense. [There is a 15-minute conference presentation of the paper here](https://zenodo.org/record/4661956#.YUrgsvkzabg).

LeanTest.Net can *help you* with *how to produce the evidence*. Note that it can *help you*, it **cannot** *produce the evidence* for you since testing is inherently a human effort.

The idea is that you use your experience, your business stakeholder knowledge and your *I-want-break-things* super power to produce a test plan document. A test plan will contain a set of tables which outline relevant combinations and a test case name for each test case which you intend to implement.

You don't need LeanTest.Net to write tooling which ties test case tables to each test run, as you simply take output from each test run and merge it with test plan documents, then generate new documents with the combined information.

For example, with the test case table below ([from the GitHub repo which accompanies the above mentioned paper](https://github.com/belgaard/OrdersExample/blob/master/doc/TestPlans/Orders.md)), a tool could simply parse the *Test case* column, find e.g. the outcome of the `PostOrderMustReportErrorWhenInvalidId` test method in a given test run, then present the combined information in a report. We may not have implementations of all four test cases at any given time during iterative development, but the tool can give us fast feedback on what we are still missing.

| Id       | Price    | Amount   | Duration | Test case |
| -------- | -------- | -------- | -------- | -------- |
| ~Invalid | Valid    | Valid    | Valid    | PostOrderMustReportErrorWhenInvalidId |
| Valid    | ~Invalid | Valid    | Valid    | PostOrderMustReportErrorWhenInvalidPrice |
| Valid    | Valid    | ~Invalid | Valid    | PostOrderMustReportErrorWhenInvalidAmount |
| Valid    | Valid    | Valid    | ~Invalid | PostOrderMustReportErrorWhenInvalidDuration |

## The LeanTest.Net attributes

The attributes of LeanTest.Net provide a simple means to augment generated documents.

### The `TestScenarioId` Attribute

A *test scenario* is essentially a headline for a group of test cases.

If you define scenarios in a table in a test plan, you can assign a scenario to a test method. Tooling can then extract the information and e.g. group test cases under each scenario in a generated report. [Here is an example of a scenario table](https://github.com/belgaard/OrdersExample/blob/master/doc/TestPlans/Orders.md),

| Scenario                   | Category                 | Comments | Scenario ID     |
| -------------------------- | ------------------------ | -------- | --------------- |
| Simple input parameter validation | InputValidation |          | Input |
| Advanced input parameter validation | CoreFunctionality |          | AdvancedInput |
| Mapping combinations of input parameters for QTE and Tbl | CoreFunctionality        |          | Core |

Define scenarios with IDs in test plan(s), then put a reference to these on each test method by attributing with the `TestScenarioId` Attribute, e.g. as,

```csharp
using LeanTest.Attribute;
// ...
[TestMethod, TestScenarioId("Input")]
public void PostOrderMustReportErrorWhenInvalidId()
{
// ...
}
```

The scenario ID is supposed to be unique within a certain context, typically a single Git repository. It is recommended to use information bearing IDs such as generic `Core`, `Input` or specific references to areas of business functionality.

### The `TestTag` Attribute

When working incrementally, you will often find that you have decided on a set of test cases but you have not yet implemented the equivalent test methods.

It is a fair assumption that test tools will flag missing test methods as *you are not there yet*. 

An explicit way to specify that test cases which have been agreed upon are not implemented yet can be documented in code with a placeholder test method with a _test tag_,

```csharp
using LeanTest.Attribute;
using static LeanTest.Attribute.TestTagAttribute;
// ...
[TestMethod, TestScenarioId("Core"), TestTag(NotImplemented)]
public void PostOrderMustBuyWhenAssetIsTradable()
{
}
```

The above example is a special case of using the `TestTag` attribute, giving it a pre-defined constant `NotImplemented`. Test tooling could *ensure* that all test cases in test plans have equivalent test methods by generating such placeholder tests for what may be missing.

In general, you can pass any string to the attribute and then interpret it as you like in your tooling.

#### The `TestDescription` Attribute

Comments can be added to test cases using the `TestDescription` attribute on test methods,

```csharp
using LeanTest.Attribute;
// ...
[TestMethod, TestScenarioId("Core"), TestDescription("This will go into the comments column for this test in the test report.")]
public void PostOrderMustSellWhenAssetIsTradable()
{
// ...
}
```

The test tooling is supposed to put the description in a column in a test report, e.g. by adding or augmenting a *Description* column in test case tables found in source test plan documents.
