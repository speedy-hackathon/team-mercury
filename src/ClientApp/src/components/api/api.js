export const restartGameRequest = async () => {
    const responce = await fetch('https://localhost:5001/api/restart', {method:"POST"});
    return responce;
}
