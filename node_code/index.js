exports.handler =  async function(event, context) {
    console.log("EVENT: \n" + JSON.stringify(event, null, 2))
    console.log("CONTEXT: \n" + JSON.stringify(context, null, 2))
    return context.logStreamName
}