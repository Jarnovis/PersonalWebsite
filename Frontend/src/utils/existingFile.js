export default async function existingFile(filepath) {
    try {
        return (await fetch(filepath, {method : 'HEAD'})).ok;
    }
    catch (error) {
        console.log('Error loading file: ', error);
        return false;
    }
}