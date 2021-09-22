# Producing Evidence

[Daniel-Terhorst North’s definines](https://dannorth.net/2021/07/26/we-need-to-talk-about-testing/), 

> *The purpose of testing is to **increase confidence** for **stakeholders** through **evidence**.*

When confronted with that definition, many people ask "*what* is meant by ***evidence*** and *how* do I produce that?"

[There is a paper which offers a possible answer to *what evidence is*](https://ieeexplore.ieee.org/document/9440154). The paper argues that a healthy dose of BDD thinking leads us to boil down the process of testing into identifying values which then can be managed with a combination of combinatorial testing, knowledge about business value and common sense. [There is a 15-minute conference presentation of the paper here](https://zenodo.org/record/4661956#.YUrgsvkzabg).

LeanTest.Net can *help you* with the *how to produce evidence* part. Note that it can *help you*, it **cannot** *produce the evidence* for you since testing is inherently a human effort.

The idea is that you use your experience, your business knowledge and your *I-want-break-things* super power to produce a test plan document. A test plan will contain a set of tables which outline relevant combinations and a test case name for each test case which you intend to implement.

You don't need LeanTest.Net to write tooling which ties test case tables to each test run, as you simply take output from each test run and merge it with test plan documents, then generate new documents with the combined information. 

For example, with the test case table below ([from the GitHub repo which accompanies the above mentioned paper](https://github.com/belgaard/OrdersExample/blob/master/doc/TestPlans/Orders.md)), a tool could simply parse the *Test case* column, find e.g. the outcome of the `PostOrderMustReportErrorWhenInvalidId` test method in a given test run, then present the combined information in a report. We may not have implementations of all four test cases at any given time during iterative development, but the tool can give us fast feedback on what we are still missing.

| Id       | Price    | Amount   | Duration | Test case |
| -------- | -------- | -------- | -------- | -------- |
| ~Invalid | Valid    | Valid    | Valid    | PostOrderMustReportErrorWhenInvalidId |
| Valid    | ~Invalid | Valid    | Valid    | PostOrderMustReportErrorWhenInvalidPrice |
| Valid    | Valid    | ~Invalid | Valid    | PostOrderMustReportErrorWhenInvalidAmount |
| Valid    | Valid    | Valid    | ~Invalid | PostOrderMustReportErrorWhenInvalidDuration |

The attributes of LeanTest.Net provide a simple means to augment such generated documents.

## The LeanTest.Net attributes

