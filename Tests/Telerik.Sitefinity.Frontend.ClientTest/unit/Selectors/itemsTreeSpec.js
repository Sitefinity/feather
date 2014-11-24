define('items tree', function  () {
	describe('sfTreeHelper service', function  () {
		it('[GeorgiMateev] / should construct tree to an item in third level.', function  () {
			var flatPredecessors = [
				//First level
				{ Id: '1.1', ParentId: '0'}, { Id: '1.2', ParentId: '0'}, { Id: '1.3', ParentId: '0'},

				//Second level
				{ Id: '2.1', ParentId: '1.2'}, { Id: '2.2', ParentId: '1.2'}, { Id: '2.3', ParentId: '1.2'}

				//Third level
				{ Id: '3.1', ParentId: '2.2'}, { Id: '3.2', ParentId: '2.2'}, { Id: '3.3', ParentId: '2.2'}];
		});
	});
});